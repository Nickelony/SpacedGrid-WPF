using System.Windows;

namespace SpacedGridControl.Demo;

public partial class MainWindow : Window
{
	public MainWindow()
		=> InitializeComponent();

	private void RowSpacingSliderPropertyChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		spacedGrid.RowSpacing = (double)e.NewValue!;

		if (textBlock_RowSpacing is not null)
			textBlock_RowSpacing.Text = e.NewValue.ToString();
	}

	private void ColumnSpacingSliderPropertyChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		spacedGrid.ColumnSpacing = (double)e.NewValue!;

		if (textBlock_ColumnSpacing is not null)
			textBlock_ColumnSpacing.Text = e.NewValue.ToString();
	}
}
