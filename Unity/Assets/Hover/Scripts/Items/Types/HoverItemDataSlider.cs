﻿using System;
using UnityEngine;

namespace Hover.Items.Types {

	/*================================================================================================*/
	[Serializable]
	public class HoverItemDataSlider : SelectableItemDataFloat, ISliderItemData {

		public Func<ISliderItemData, string> GetFormattedLabel { get; set; }

		[SerializeField]
		private string _LabelFormat = "{0}: {1:N1}";

		[SerializeField]
		private int _Ticks = 0;

		[SerializeField]
		private int _Snaps = 0;

		[SerializeField]
		private float _RangeMin = -100;

		[SerializeField]
		private float _RangeMax = 100;

		[SerializeField]
		private bool _AllowJump = false;

		[SerializeField]
		private SliderFillType _FillStartingPoint = SliderFillType.Zero;
		
		private float? vHoverValue;
		private string vPrevLabel;
		private string vPrevLabelFormat;
		private float vPrevSnappedRangeValue;
		private string vPrevValueToLabel;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public HoverItemDataSlider() {
			_Value = 0.5f;

			GetFormattedLabel = (s => {
				if ( s.Label == vPrevLabel && s.LabelFormat == vPrevLabelFormat &&
						s.SnappedRangeValue == vPrevSnappedRangeValue ) {
					return vPrevValueToLabel;
				}
				
				vPrevLabel = s.Label;
				vPrevLabelFormat = s.LabelFormat;
				vPrevSnappedRangeValue = s.SnappedRangeValue;
				vPrevValueToLabel = string.Format(vPrevLabelFormat,
					vPrevLabel, vPrevSnappedRangeValue); //GC_ALLOC
				return vPrevValueToLabel;
			});
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public string LabelFormat {
			get { return _LabelFormat; }
			set { _LabelFormat = value; }
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public int Ticks {
			get { return _Ticks; }
			set { _Ticks = value; }
		}

		/*--------------------------------------------------------------------------------------------*/
		public int Snaps {
			get { return _Snaps; }
			set { _Snaps = value; }
		}

		/*--------------------------------------------------------------------------------------------*/
		public float RangeMin {
			get { return _RangeMin; }
			set { _RangeMin = value; }
		}

		/*--------------------------------------------------------------------------------------------*/
		public float RangeMax {
			get { return _RangeMax; }
			set { _RangeMax = value; }
		}

		/*--------------------------------------------------------------------------------------------*/
		public bool AllowJump {
			get { return _AllowJump; }
			set { _AllowJump = value; }
		}

		/*--------------------------------------------------------------------------------------------*/
		public SliderFillType FillStartingPoint {
			get { return _FillStartingPoint; }
			set { _FillStartingPoint = value; }
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public override void DeselectStickySelections() {
			Value = SnappedValue;
			base.DeselectStickySelections();
		}

		/*--------------------------------------------------------------------------------------------*/
		public override float Value {
			get {
				return base.Value;
			}
			set {
				base.Value = Math.Max(0, Math.Min(1, value));
			}
		}
		
		
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public float RangeValue {
			get {
				return Value*(RangeMax-RangeMin)+RangeMin;
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public float SnappedValue {
			get {
				return CalcSnappedValue(Value);
			}
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public float SnappedRangeValue {
			get {
				return SnappedValue*(RangeMax-RangeMin)+RangeMin;
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public float? HoverValue {
			get {
				return vHoverValue;
			}
			set {
				if ( value == null ) {
					vHoverValue = null;
					return;
				}

				vHoverValue = Math.Max(0, Math.Min(1, (float)value));
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public float? SnappedHoverValue {
			get {
				if ( HoverValue == null ) {
					return null;
				}

				return CalcSnappedValue((float)HoverValue);
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override bool UsesStickySelection() {
			return true;
		}

		/*--------------------------------------------------------------------------------------------*/
		private float CalcSnappedValue(float pValue) {
			if ( Snaps < 2 ) {
				return pValue;
			}

			int s = Snaps-1;
			return (float)Math.Round(pValue*s)/s;
		}

	}

}
