using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SpacedGridControl;

public class SpacedGrid : Grid
{
	public static readonly DependencyProperty RowSpacingProperty;
	public static readonly DependencyProperty ColumnSpacingProperty;

	public double RowSpacing
	{
		get => (double)GetValue(RowSpacingProperty);
		set => SetValue(RowSpacingProperty, value);
	}

	public double ColumnSpacing
	{
		get => (double)GetValue(ColumnSpacingProperty);
		set => SetValue(ColumnSpacingProperty, value);
	}

	static SpacedGrid()
	{
		DefaultStyleKeyProperty.OverrideMetadata(typeof(SpacedGrid), new FrameworkPropertyMetadata(typeof(SpacedGrid)));

		RowSpacingProperty = DependencyProperty.Register(nameof(RowSpacing), typeof(double), typeof(SpacedGrid), new UIPropertyMetadata(3.0));
		ColumnSpacingProperty = DependencyProperty.Register(nameof(ColumnSpacing), typeof(double), typeof(SpacedGrid), new UIPropertyMetadata(3.0));
	}

	protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		base.OnPropertyChanged(e);

		if (e.Property == RowSpacingProperty || e.Property == ColumnSpacingProperty)
			RecalculateMarginsOfChildren();
	}

	protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
	{
		base.OnVisualChildrenChanged(visualAdded, visualRemoved);

		if (visualAdded is UIElement element)
			element.IsVisibleChanged += Element_IsVisibleChanged; // If the element isn't visible,
																  // we won't be able to get the row / column it's in,
																  // so we have to wait until it's visible,
																  // then immediately unbind the event handler.
	}

	private void Element_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		var element = sender as UIElement;
		element.IsVisibleChanged -= Element_IsVisibleChanged;

		RecalculateMarginsOfChildren();
	}

	private void RecalculateMarginsOfChildren()
	{
		for (int i = 0; i < Children.Count; i++)
		{
			UIElement child = Children.Cast<UIElement>().ElementAt(i);

			if (child is not SpacedGridItem item)
			{
				item = new SpacedGridItem();

				int childIndex = Children.IndexOf(child),
					row = GetRow(child),
					column = GetColumn(child),
					rowSpan = GetRowSpan(child),
					columnSpan = GetColumnSpan(child);

				Children.Remove(child);

				item.Child = child;
				SetRow(item, row);
				SetColumn(item, column);
				SetRowSpan(item, rowSpan);
				SetColumnSpan(item, columnSpan);

				Children.Insert(childIndex, item);
			}

			RecalculateMargin(item);
		}
	}

	private void RecalculateMargin(SpacedGridItem item)
	{
		int row = GetRow(item),
			column = GetColumn(item),
			rowSpan = GetRowSpan(item),
			columnSpan = GetColumnSpan(item),
			rowCount = RowDefinitions.Count,
			columnCount = ColumnDefinitions.Count;

		double halfRowSpacing = RowSpacing / 2,
			halfColumnSpacing = ColumnSpacing / 2;

		double left, top, right, bottom;

		left = column == 0 ? 0 : halfColumnSpacing;
		top = row == 0 ? 0 : halfRowSpacing;
		right = column + columnSpan == columnCount ? 0 : halfColumnSpacing;
		bottom = row + rowSpan == rowCount ? 0 : halfRowSpacing;

		item.Margin = new Thickness(left, top, right, bottom);
	}
}
