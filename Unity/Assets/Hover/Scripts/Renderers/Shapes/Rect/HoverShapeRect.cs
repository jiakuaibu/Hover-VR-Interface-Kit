using Hover.Items;
using Hover.Layouts.Rect;
using Hover.Renderers.Utils;
using Hover.Utils;
using UnityEngine;

namespace Hover.Renderers.Shapes.Rect {

	/*================================================================================================*/
	public class HoverShapeRect : HoverShape, IRectLayoutable {
		
		public const string SizeXName = "SizeX";
		public const string SizeYName = "SizeY";

		[DisableWhenControlled(RangeMin=0)]
		public float SizeX = 0.1f;
		
		[DisableWhenControlled(RangeMin=0)]
		public float SizeY = 0.1f;

		private Plane vWorldPlane;
		private float vPrevSizeX;
		private float vPrevSizeY;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public override Vector3 GetCenterWorldPosition() {
			return gameObject.transform.position;
		}

		/*--------------------------------------------------------------------------------------------*/
		public override Vector3 GetNearestWorldPosition(Vector3 pFromWorldPosition) {
			return RendererUtil.GetNearestWorldPositionOnRectangle(
				pFromWorldPosition, gameObject.transform, SizeX, SizeY);
		}

		/*--------------------------------------------------------------------------------------------*/
		public override Vector3 GetNearestWorldPosition(Ray pFromWorldRay, out RaycastResult pRaycast) {
			pRaycast.WorldPosition = 
				RendererUtil.GetNearestWorldPositionOnPlane(pFromWorldRay, vWorldPlane);
			pRaycast.WorldRotation = transform.rotation;
			pRaycast.WorldPlane = vWorldPlane;
			return GetNearestWorldPosition(pRaycast.WorldPosition);
		}

		/*--------------------------------------------------------------------------------------------*/
		public override float GetSliderValueViaNearestWorldPosition(Vector3 pNearestWorldPosition, 
										Transform pSliderContainerTx, HoverShape pHandleButtonShape) {
			HoverShapeRect buttonShapeRect = (pHandleButtonShape as HoverShapeRect);

			if ( buttonShapeRect == null ) {
				Debug.LogError("Expected slider handle to have a '"+typeof(HoverShapeRect).Name+
					"' component attached to it.", this);
				return 0;
			}
			
			Vector3 nearLocalPos = pSliderContainerTx.InverseTransformPoint(pNearestWorldPosition);
			float halfTrackSizeY = (SizeY-buttonShapeRect.SizeY)/2;
			return Mathf.InverseLerp(-halfTrackSizeY, halfTrackSizeY, nearLocalPos.y);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void SetRectLayout(float pSizeX, float pSizeY, ISettingsController pController) {
			Controllers.Set(SizeXName, pController);
			Controllers.Set(SizeYName, pController);

			SizeX = pSizeX;
			SizeY = pSizeY;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public override void TreeUpdate() {
			base.TreeUpdate();

			vWorldPlane = RendererUtil.GetWorldPlane(gameObject.transform);

			DidSettingsChange = (
				DidSettingsChange ||
				SizeX != vPrevSizeX ||
				SizeY != vPrevSizeY
			);

			UpdateShapeRectChildren();

			vPrevSizeX = SizeX;
			vPrevSizeY = SizeY;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void UpdateShapeRectChildren() {
			if ( !ControlChildShapes ) {
				return;
			}

			TreeUpdater tree = GetComponent<TreeUpdater>();

			for ( int i = 0 ; i < tree.TreeChildrenThisFrame.Count ; i++ ) {
				TreeUpdater child = tree.TreeChildrenThisFrame[i];
				HoverShapeRect childRect = child.GetComponent<HoverShapeRect>();

				if ( childRect == null ) {
					continue;
				}

				childRect.Controllers.Set(SizeXName, this);
				childRect.Controllers.Set(SizeYName, this);

				childRect.SizeX = SizeX;
				childRect.SizeY = SizeY;
			}
		}

	}

}
