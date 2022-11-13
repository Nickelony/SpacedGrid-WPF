using SpacedGridControl.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace SpacedGridControl.Definitions
{
	public class SpacingRowDefinition : RowDefinition, ISpacingDefinition
	{
		public double Spacing
		{
			get => Height.Value;
			set => Height = new GridLength(value, GridUnitType.Pixel);
		}

		public SpacingRowDefinition(double height)
			=> Height = new GridLength(height, GridUnitType.Pixel);
	}
}
