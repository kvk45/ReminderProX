namespace ReminderProX.Controls;

public partial class CircularProgressControl : ContentView
{
    public static readonly BindableProperty ProgressProperty =
        BindableProperty.Create(
            nameof(Progress),
            typeof(double),
            typeof(CircularProgressControl),
            0.0,
            propertyChanged: OnProgressChanged);

    public static readonly BindableProperty ProgressColorProperty =
        BindableProperty.Create(
            nameof(ProgressColor),
            typeof(Color),
            typeof(CircularProgressControl),
            null,
            propertyChanged: OnColorChanged);

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public Color? ProgressColor
    {
        get => (Color?)GetValue(ProgressColorProperty);
        set => SetValue(ProgressColorProperty, value);
    }

    private readonly GraphicsView _graphicsView;
    private readonly CircularProgressDrawable _drawable;
    private Animation? _animation;

    public CircularProgressControl()
    {
        WidthRequest = 90;
        HeightRequest = 90;
        HorizontalOptions = LayoutOptions.Center;
        VerticalOptions = LayoutOptions.Center;

        _drawable = new CircularProgressDrawable();
        _graphicsView = new GraphicsView
        {
            WidthRequest = 90,
            HeightRequest = 90,
            Drawable = _drawable,
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        Content = _graphicsView;

        Application.Current?.RequestedThemeChanged += (s, a) =>
        {
            MainThread.BeginInvokeOnMainThread(UpdateVisuals);
        };

        Loaded += (s, e) => PlayAnimation();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        PlayAnimation();
    }

    private static void OnProgressChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CircularProgressControl control)
        {
            control.PlayAnimation();
        }
    }

    private static void OnColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CircularProgressControl control)
        {
            control.UpdateVisuals();
        }
    }

    private double GetNormalizedProgress()
    {
        double val = Progress;
        if (val > 1.0) val /= 100.0;
        return Math.Clamp(val, 0.0, 1.0);
    }

    private Color GetColorForProgress(double normalizedValue, bool isDark)
    {
        if (ProgressColor != null)
            return ProgressColor;

        if (normalizedValue <= 0.001)
            return isDark ? Color.FromArgb("#424242") : Color.FromArgb("#BDBDBD");

        if (normalizedValue <= 0.33)
            return Color.FromArgb("#D32F2F");
        if (normalizedValue <= 0.70)
            return Color.FromArgb("#F57C00");

        return Color.FromArgb("#2E7D32");
    }

    private void UpdateVisuals()
    {
        var currentTheme = Application.Current?.RequestedTheme ?? AppTheme.Light;
        bool isDark = currentTheme == AppTheme.Dark;

        double normalized = GetNormalizedProgress();
        _drawable.ProgressColor = GetColorForProgress(normalized, isDark);
        _drawable.BackgroundColor = isDark ? Color.FromArgb("#424242") : Color.FromArgb("#E0E0E0");
        _graphicsView.Invalidate();
    }

    public void PlayAnimation()
    {
        this.AbortAnimation("ProgressAnim");

        var currentTheme = Application.Current?.RequestedTheme ?? AppTheme.Light;
        bool isDark = currentTheme == AppTheme.Dark;

        double targetProgress = GetNormalizedProgress();
        _drawable.ProgressColor = GetColorForProgress(targetProgress, isDark);
        _drawable.BackgroundColor = isDark ? Color.FromArgb("#424242") : Color.FromArgb("#E0E0E0");

        if (targetProgress <= 0.0)
        {
            _drawable.Progress = 0.0;
            _graphicsView.Invalidate();
            return;
        }

        _animation = new Animation(v =>
        {
            _drawable.Progress = v;
            _graphicsView.Invalidate();
        }, 0.0, targetProgress, Easing.CubicOut);

        _animation.Commit(this, "ProgressAnim", 16, 400);
    }

    private class CircularProgressDrawable : IDrawable
    {
        public double Progress { get; set; } = 0.0;
        public Color ProgressColor { get; set; } = Color.FromArgb("#2E7D32");
        public Color BackgroundColor { get; set; } = Color.FromArgb("#E0E0E0");

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float strokeWidth = 9f;
            float width = dirtyRect.Width > 0 ? dirtyRect.Width : 90f;
            float height = dirtyRect.Height > 0 ? dirtyRect.Height : 90f;
            float size = Math.Min(width, height) - strokeWidth;
            float x = (width - size) / 2f;
            float y = (height - size) / 2f;

            canvas.StrokeSize = strokeWidth;
            canvas.StrokeLineCap = LineCap.Butt;

            canvas.StrokeColor = BackgroundColor;
            canvas.DrawEllipse(x, y, size, size);

            if (Progress > 0.001)
            {
                canvas.StrokeColor = ProgressColor;

                float sweepAngle = (float)(Math.Clamp(Progress, 0.0, 1.0) * 360f);

                if (sweepAngle >= 359.5f)
                {
                    canvas.DrawEllipse(x, y, size, size);
                }
                else
                {
                    float startAngle = -90f;
                    canvas.DrawArc(x, y, size, size, startAngle, startAngle + sweepAngle, false, false);
                }
            }
        }
    }
}