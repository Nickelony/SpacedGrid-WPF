using SpacedGridControl.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace SpacedGridControl.Definitions
{
	public class SpacingColumnDefinition : ColumnDefinition, ISpacingDefinition
	{
		public double Spacing
		{
			get => Width.Value;
			set => Width = new GridLength(value, GridUnitType.Pixel);
		}

		public SpacingColumnDefinition(double width)
			=> Width = new GridLength(width, GridUnitType.Pixel);
	}
}
