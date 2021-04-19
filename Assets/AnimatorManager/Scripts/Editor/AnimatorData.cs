using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using ReorderableList = UnityEditorInternal.ReorderableList;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;

namespace AnimatorManager.Scripts.Editor {

	[CreateAssetMenu(fileName = "New Animator Manager Data", menuName = "Animator Manager/Animator Data", order = 0)]
	public class AnimatorData : ScriptableObject {
		[SerializeField]public AnimatorController referenceAnimator;

		public List<AnimatorLayer> layers = new List<AnimatorLayer>();
		public List<AnimatorInput> inputs = new List<AnimatorInput>();

		public ReorderableList layerlist;
		public ReorderableList inputlist;
		
		public Vector2 tab1scroll;
		public Vector2 tab2scroll;

		private void OnEnable() {
			
			// Testing entries
			/*
			layers.Add(new AnimatorLayer("Test 1", new List<AnimatorState>()));
			layers.Add(new AnimatorLayer("Test 2", new List<AnimatorState>()));
			layers.Add(new AnimatorLayer("Test 3", new List<AnimatorState>()));
			
			inputs.Add(new AnimatorInput());
			inputs.Add(new AnimatorInput());
			inputs.Add(new AnimatorInput());
			*/
			

			inputlist = new ReorderableList(inputs, typeof(AnimatorInput));
			inputlist.drawElementCallback += DrawInputElementCallback;
			inputlist.elementHeightCallback += InputElementHeightCallback;
			inputlist.onAddCallback += ONInputAddCallback;
			inputlist.headerHeight = 0;

			layerlist = new ReorderableList(layers, typeof(AnimatorLayer));
			layerlist.drawElementCallback += DrawLayerElementCallback;
			layerlist.elementHeightCallback += LayerElementHeightCallback;
			layerlist.onAddCallback += ONLayerAddCallback;
			layerlist.headerHeight = 0;
		}

		// ####################### INPUT ################### //

        private float InputElementHeightCallback(int index) {
            var entity = inputs[index];

            float height = EditorGUIUtility.singleLineHeight * 1.25f;
            
            if (entity.isNotCollapsed) {
                height += EditorGUIUtility.singleLineHeight * 1.25f;
                height += EditorGUIUtility.singleLineHeight * 1.25f;
                if (entity.type != AnimatorControllerParameterType.Trigger) {
                    height += EditorGUIUtility.singleLineHeight * 1.25f;
                }

                if (entity.type == AnimatorControllerParameterType.Float || entity.type == AnimatorControllerParameterType.Int) {
                    height += entity.OptionsListHeight;
                }
                
                height += EditorGUIUtility.singleLineHeight * 0.5f;
            }

            return height;
        }

        private void DrawInputElementCallback(Rect rect, int index, bool isactive, bool isfocused) {
            var entity = inputs[index];
            
            rect.y += 2;
            rect.x += 12;
            rect.width -= 12;
            Rect _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            entity.isNotCollapsed = EditorGUI.Foldout(_rect, entity.isNotCollapsed, "Name", true);
            _rect.x += 100;
            _rect.width = rect.width - 100;
            entity.Name = EditorGUI.TextField(_rect, entity.Name);

            if (entity.isNotCollapsed) {

                rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                entity.parameterName = EditorGUI.TextField(_rect, "Parameter Name", entity.parameterName);
                
                
                rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                entity.type = (AnimatorControllerParameterType)EditorGUI.EnumPopup(_rect, "Type", entity.type);
                
                
                rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                //EditorGUI.Popup(_rect, "Default Value", entity.defaultOptionIndex, entity.GetOptionNames()); // Dropdown for default value
                switch (entity.type) {
                    case AnimatorControllerParameterType.Float:
                        _rect.width -= 100;
                        entity.defaultOptionIndex = EditorGUI.Popup(_rect, "Default Option", entity.defaultOptionIndex, entity.GetOptionNames()); // Dropdown for default value
                        _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                        _rect.width = 90;
                        _rect.x = rect.x + (rect.width - 90);
                        entity.defaultOptionIndex = EditorGUI.IntField(_rect, entity.defaultOptionIndex);
                        break;
                    case AnimatorControllerParameterType.Int:
                        _rect.width -= 100;
                        entity.defaultOptionIndex = EditorGUI.Popup(_rect, "Default Option", entity.defaultOptionIndex, entity.GetOptionNames()); // Dropdown for default value
                        _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                        _rect.width = 90;
                        _rect.x = rect.x + (rect.width - 90);
                        entity.defaultOptionIndex = EditorGUI.IntField(_rect, entity.defaultOptionIndex);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        entity.defaultBool = EditorGUI.Toggle(_rect, "Default Value", entity.defaultBool);
                        break;
                    default:
                        break;
                }
                if (entity.optionsRList != null && (entity.type == AnimatorControllerParameterType.Float || entity.type == AnimatorControllerParameterType.Int)) {
                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    _rect = new Rect(rect.x, rect.y, rect.width, entity.OptionsListHeight);
                    entity.optionsRList.DoList(_rect);
                }
            }
        }

        private void ONInputAddCallback(ReorderableList list) {
	        AnimatorInput input = new AnimatorInput(this);
	        inputs.Add(input);
        }
        
        // ############### LAYERS ################ //

        private float LayerElementHeightCallback(int index) {
            var entity = layers[index];

            float height = EditorGUIUtility.singleLineHeight * 1.25f;
            
            if (entity.isNotCollapsed) {
                height += EditorGUIUtility.singleLineHeight * 1.25f;
                height += EditorGUIUtility.singleLineHeight * 1.25f;
                height += entity.statesRList.GetHeight();
                
                height += EditorGUIUtility.singleLineHeight * 0.5f;
            }

            return height;
        }

        private void DrawLayerElementCallback(Rect rect, int index, bool isactive, bool isfocused) {
            var entity = layers[index];
            
            rect.y += 2;
            rect.x += 12;
            rect.width -= 12;
            Rect _rect = new Rect(rect.x, rect.y, rect.width - 200, EditorGUIUtility.singleLineHeight);

            entity.isNotCollapsed = EditorGUI.Foldout(_rect, entity.isNotCollapsed, entity.Name, true);

            _rect.x += _rect.width + 110;
            _rect.width = 70;
            EditorGUI.LabelField(_rect, "Override");
            
            _rect.x += _rect.width;
            _rect.width = 30;
            entity.overrideExistingLayers = EditorGUI.Toggle(_rect, entity.overrideExistingLayers);

            if (entity.isNotCollapsed) {
	            
	            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
	            _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
	            entity.Name = EditorGUI.TextField(_rect, "Name", entity.name);

	            int previousPrimaryInputIndex = entity.primaryInputIndex;
	            
                rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                entity.primaryInputIndex = EditorGUI.Popup(_rect, "Primary Input", entity.primaryInputIndex, GetInputNames());

                if (previousPrimaryInputIndex != entity.primaryInputIndex) {
	                entity.SetStatesFromInputOptions(inputs[entity.primaryInputIndex].options);
                }

                rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                if (entity.states != null) {
                    entity.statesRList.DoList(_rect);
                }
            }
        }

        private void ONLayerAddCallback(ReorderableList list) {
	        AnimatorLayer layer = new AnimatorLayer(this);
	        if (inputs.Count > 0) {
		        layer.SetStatesFromInputOptions(inputs[0].options);
	        }
	        layers.Add(layer);
        }
        
        // ####################### other ######################### //

		public string[] GetInputNames() {
			return inputs.Select(input => input.Name).ToArray();
		}

		public void LoadAnimator(AnimatorController anim) {
			referenceAnimator = anim;
			foreach (var parameter in referenceAnimator.parameters) {
				var input = new AnimatorInput(this);
				input.parameterName = parameter.name;
				input.type = parameter.type;
				input.defaultFloat = parameter.defaultFloat;
				input.defaultInt = parameter.defaultInt;
				input.defaultBool = parameter.defaultBool;
				inputs.Add(input);
			}
		}

		public void Save() {
			//Save Data into Animator
			Debug.Log("This Method is not Implemented yet!");
			
			// would be to create new data asset for new animator
			//AssetDatabase.CreateAsset(this, AssetDatabase.GetAssetPath(this));
		}

		public void Reset() {
			Clear();
			LoadAnimator(referenceAnimator);
		}

		public void Clear() {
			inputs.Clear();
			layers.Clear();
		}
	}
}