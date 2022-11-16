using SpacedGridControl.Definitions;
using SpacedGridControl.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SpacedGridControl
{
	public class SpacedGrid : Grid
	{
		#region Properties

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

		/// <summary>
		/// Returns an enumerable of all the grid's row definitions, <u>excluding</u> spacing rows.
		/// </summary>
		public IEnumerable<RowDefinition> UserDefinedRowDefinitions =>
			from definition in RowDefinitions
			where !(definition is ISpacingDefinition)
			select definition;

		/// <summary>
		/// Returns an enumerable of all the grid's column definitions, <u>excluding</u> spacing columns.
		/// </summary>
		public IEnumerable<ColumnDefinition> UserDefinedColumnDefinitions =>
			from definition in ColumnDefinitions
			where !(definition is ISpacingDefinition)
			select definition;

		#endregion Properties

		#region Construction

		static SpacedGrid()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SpacedGrid), new FrameworkPropertyMetadata(typeof(SpacedGrid)));

			RowSpacingProperty = DependencyProperty.Register(nameof(RowSpacing), typeof(double), typeof(SpacedGrid), new UIPropertyMetadata(3.0));
			ColumnSpacingProperty = DependencyProperty.Register(nameof(ColumnSpacing), typeof(double), typeof(SpacedGrid), new UIPropertyMetadata(3.0));
		}

		#endregion Construction

		#region Override methods

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);

			UpdateSpacedRows();
			UpdateSpacedColumns();

			if (visualAdded is UIElement element)
				element.IsVisibleChanged += Element_IsVisibleChanged; // If the element isn't visible,
																	  // we won't be able to get the row / column it's in,
																	  // so we have to wait until it's visible,
																	  // then immediately unbind the event handler.
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			switch (e.Property.Name)
			{
				case nameof(RowSpacing):
					RecalculateRowSpacing();
					break;

				case nameof(ColumnSpacing):
					RecalculateColumnSpacing();
					break;
			}
		}

		#endregion Override methods

		#region Events

		private void Element_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var element = sender as UIElement;
			element.IsVisibleChanged -= Element_IsVisibleChanged;

			SetRow(element, GetRow(element) * 2); // 1 -> 2 or 2 -> 4
			SetRowSpan(element, (GetRowSpan(element) * 2) - 1); // 2 -> 3 or 3 -> 5

			SetColumn(element, GetColumn(element) * 2); // 1 -> 2 or 2 -> 4
			SetColumnSpan(element, (GetColumnSpan(element) * 2) - 1); // 2 -> 3 or 3 -> 5
		}

		#endregion Events

		#region Other methods

		private void UpdateSpacedRows()
		{
			var userRowDefinitions = UserDefinedRowDefinitions.ToList(); // User-defined rows (e.g. the ones defined in XAML files)
			var actualRowDefinitions = new List<RowDefinition>(); // User-defined + spacing rows

			int currentUserDefinition = 0,
				currentActualDefinition = 0;

			while (currentUserDefinition < userRowDefinitions.Count)
			{
				if (currentActualDefinition % 2 == 0) // Even rows are user-defined rows (0, 2, 4, 6, 8, 10, ...)
				{
					actualRowDefinitions.Add(userRowDefinitions[currentUserDefinition]);
					currentUserDefinition++;
				}
				else // Odd rows are spacing rows (1, 3, 5, 7, 9, 11, ...)
					actualRowDefinitions.Add(new SpacingRowDefinition(RowSpacing));

				currentActualDefinition++;
			}

			RowDefinitions.Clear();
			actualRowDefinitions.ForEach(row => RowDefinitions.Add(row));
		}

		private void UpdateSpacedColumns()
		{
			var userColumnDefinitions = UserDefinedColumnDefinitions.ToList(); // User-defined columns (e.g. the ones defined in XAML files)
			var actualColumnDefinitions = new List<ColumnDefinition>(); // User-defined + spacing columns

			int currentUserDefinition = 0,
				currentActualDefinition = 0;

			while (currentUserDefinition < userColumnDefinitions.Count)
			{
				if (currentActualDefinition % 2 == 0) // Even columns are user-defined columns (0, 2, 4, 6, 8, 10, ...)
				{
					actualColumnDefinitions.Add(userColumnDefinitions[currentUserDefinition]);
					currentUserDefinition++;
				}
				else // Odd columns are spacing columns (1, 3, 5, 7, 9, 11, ...)
					actualColumnDefinitions.Add(new SpacingColumnDefinition(ColumnSpacing));

				currentActualDefinition++;
			}

			ColumnDefinitions.Clear();
			actualColumnDefinitions.ForEach(col => ColumnDefinitions.Add(col));
		}

		private void RecalculateRowSpacing()
		{
			foreach (ISpacingDefinition spacingRow in RowDefinitions.OfType<ISpacingDefinition>())
				spacingRow.Spacing = RowSpacing;
		}

		private void RecalculateColumnSpacing()
		{
			foreach (ISpacingDefinition spacingColumn in ColumnDefinitions.OfType<ISpacingDefinition>())
				spacingColumn.Spacing = ColumnSpacing;
		}

		#endregion Other methods
	}
}
