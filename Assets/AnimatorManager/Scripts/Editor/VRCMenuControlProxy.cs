﻿using System;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

#if VRC_SDK_VRCSDK3
namespace AnimatorManager.Scripts.Editor {
	public class VRCMenuControlProxy : VRCExpressionsMenu.Control {

		public VRCExpressionsMenu.Control reference;
		public VRCExpressionsMenuWrapper subMenuWrapper;

		public bool isExpanded = true;

		public VRCMenuControlProxy(VRCExpressionsMenu.Control reference) {
			this.reference = reference;
		}

		public Texture2D Icon {
			set => reference.icon = value;
			get => reference.icon;
		}

		public Label[] Labels {
			set => reference.labels = value;
			get => reference.labels;
		}

		public string Name {
			set => reference.name = value;
			get => reference.name;
		}

		private VRCParameterProxy _parameterWrapper = new VRCParameterProxy();

		public VRCParameterProxy ParameterProperty {
			set => reference.parameter = _parameterWrapper = value;
			get => _parameterWrapper;
		}

		public VRCExpressionsMenu SubMenu {
			set => reference.subMenu = value;
			get => reference.subMenu;
		}

		private VRCParameterProxy[] _subParameterWrapper;
		
		public VRCParameterProxy[] SubParameters {
			set => reference.subParameters = _subParameterWrapper = value;
			get => _subParameterWrapper;
		}

		public ControlType Type {
			set => reference.type = value;
			get => reference.type;
		}

		public float Value {
			set => reference.value = value;
			get => reference.value;
		}
		
		public class VRCParameterProxy : Parameter {
			public int inputIndex;

			public VRCParameterProxy() {
			}

			public VRCParameterProxy(Parameter original) {
				name = original.name;
			}
		}
	}
}
#endif