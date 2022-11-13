using SpacedGridControl.Definitions;
using SpacedGridControl.Interfaces;
using System;
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

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			UpdateSpacedRows();
			UpdateSpacedColumns();

			UpdateChildren();
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property.Name.Equals(nameof(RowDefinitions), StringComparison.OrdinalIgnoreCase))
				UpdateSpacedRows();
			else if (e.Property.Name.Equals(nameof(ColumnDefinitions), StringComparison.OrdinalIgnoreCase))
				UpdateSpacedColumns();
			else if (e.Property.Name.Equals(nameof(Children), StringComparison.OrdinalIgnoreCase))
				UpdateChildren();
			else if (e.Property.Name.Equals(nameof(RowSpacing), StringComparison.OrdinalIgnoreCase))
				RecalculateRowSpacing();
			else if (e.Property.Name.Equals(nameof(ColumnSpacing), StringComparison.OrdinalIgnoreCase))
				RecalculateColumnSpacing();
		}

		#endregion Override methods

		#region Other methods

		private void UpdateSpacedRows()
		{
			var userRowDefinitions = new List<RowDefinition>(); // User-defined rows (e.g. the ones defined in XAML files)
			userRowDefinitions.AddRange(RowDefinitions.Where(x => !(x is ISpacingDefinition))); // Exclude spacing rows

			var actualRowDefinitions = new List<RowDefinition>(); // User-defined + spacing rows

			int currentUserDefinition = 0, currentActualDefinition = 0;

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
			var userColumnDefinitions = new List<ColumnDefinition>(); // User-defined columns (e.g. the ones defined in XAML files)
			userColumnDefinitions.AddRange(ColumnDefinitions.Where(x => !(x is ISpacingDefinition))); // Exclude spacing columns

			var actualColumnDefinitions = new List<ColumnDefinition>(); // User-defined + spacing columns

			int currentUserDefinition = 0, currentActualDefinition = 0;

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

		/// <summary>
		/// Updates the following parameters of passed children, so they match the new Row and Column definitions:<br />
		/// <c>Grid.Row</c><br />
		/// <c>Grid.Column</c><br />
		/// <c>Grid.RowSpan</c><br />
		/// <c>Grid.ColumnSpan</c>
		/// </summary>
		private void UpdateChildren()
		{
			foreach (Control child in Children)
			{
				SetRow(child, GetRow(child) * 2); // 1 -> 2 or 2 -> 4
				SetRowSpan(child, (GetRowSpan(child) * 2) - 1); // 2 -> 3 or 3 -> 5

				SetColumn(child, GetColumn(child) * 2); // 1 -> 2 or 2 -> 4
				SetColumnSpan(child, (GetColumnSpan(child) * 2) - 1); // 2 -> 3 or 3 -> 5
			}
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
