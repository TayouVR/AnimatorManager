using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using ReorderableList = UnityEditorInternal.ReorderableList;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;

#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.ScriptableObjects;
#endif

namespace AnimatorManager.Scripts.Editor {

	//[CreateAssetMenu(fileName = "New Animator Manager Data", menuName = "Animator Manager/Animator Data", order = 0)]
	public class Data : ScriptableObject {
		[SerializeField]public AnimatorController referenceAnimator;

		public List<AnimatorLayer> layers = new List<AnimatorLayer>();
		public List<Input> inputs = new List<Input>();

		public ReorderableList layerlist;
		public ReorderableList inputlist;
		
		public Vector2 tab1scroll;
		public Vector2 tab2scroll;
		public Vector2 tab4scroll;
		public Vector2 tab5scroll;

		public List<string> backupAnimatorControllers = new List<string>();

		public AnimationSavePathCat savePathCat;
		public string customSavePath;

		private VRCExpressionsMenuWrapper menuWrapper;

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
			

			inputlist = new ReorderableList(inputs, typeof(Input));
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
                    height += entity.OptionsRList.GetHeight();
                }
#if VRC_SDK_VRCSDK3
	            height += EditorGUIUtility.singleLineHeight * 1.25f;
#endif
                
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
                if (entity.previousType != entity.type) {
	                if (entity.type == AnimatorControllerParameterType.Bool) {
		                entity.options.Clear();
		                entity.options.Add(new InputOption("Off", 0, 0));
		                entity.options.Add(new InputOption("On", 1, 1));
	                }

	                entity.previousType = entity.type;
                }
                
                
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

#if VRC_SDK_VRCSDK3
	            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
	            _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
	            entity.saved = EditorGUI.Toggle(_rect, "Saved", entity.saved);
#endif
                
                if (entity.OptionsRList != null && (entity.type == AnimatorControllerParameterType.Float || entity.type == AnimatorControllerParameterType.Int)) {
                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    _rect = new Rect(rect.x, rect.y, rect.width, entity.OptionsRList.GetHeight());
                    entity.OptionsRList.DoList(_rect);
                }
            }
        }

        private void ONInputAddCallback(ReorderableList list) {
	        Input input = new Input(this);
	        inputs.Add(input);
        }
        
        // ############### LAYERS ################ //

        private float LayerElementHeightCallback(int index) {
            var entity = layers[index];

            float height = EditorGUIUtility.singleLineHeight * 1.25f;
            
            if (entity.stateMachine.isNotCollapsed) {
                height += EditorGUIUtility.singleLineHeight * 1.25f;
                height += EditorGUIUtility.singleLineHeight * 1.25f;
                height += entity.stateMachine.StatesRList.GetHeight();
                
                height += EditorGUIUtility.singleLineHeight * 0.5f;
            }

            return height;
        }

        private void DrawLayerElementCallback(Rect rect, int index, bool isactive, bool isfocused) {
            var entity = layers[index];
            DrawStateMachine(rect, entity.stateMachine);
        }

        public void DrawStateMachine(Rect rect, StateMachine entity) {
	        
	        rect.y += 2;
	        //rect.x += 12;
	        //rect.width -= 12;
	        Rect _rect = new Rect(rect.x, rect.y, rect.width - 200, EditorGUIUtility.singleLineHeight);

	        entity.isNotCollapsed = EditorGUI.Foldout(_rect, entity.isNotCollapsed, entity.Name, true);

	        GUI.enabled = entity.controllerStateMachine != null;
	        _rect.x += _rect.width + 110;
	        _rect.width = 70;
	        EditorGUI.LabelField(_rect, "Override");
            
	        _rect.x += _rect.width;
	        _rect.width = 30;
	        entity.overrideExistingStateMachine = EditorGUI.Toggle(_rect, !GUI.enabled || entity.overrideExistingStateMachine);
	        GUI.enabled = true;

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
			        entity.StatesRList.DoList(_rect);
		        }
	        }
        }

        public void DrawState(Rect rect, State entity) {
	        
	        rect.y += 2;
	        rect.height = EditorGUIUtility.singleLineHeight;
	        Rect _rect = new Rect(rect);
	        _rect.height = EditorGUIUtility.singleLineHeight;
	        _rect.width = _rect.width/2 - 10;

	        entity.isNotCollapsed = EditorGUI.Foldout(_rect, entity.isNotCollapsed, entity.Name);
			
	        _rect.x = _rect.x + _rect.width + 20;
			
	        entity.motion = (Motion)EditorGUI.ObjectField(_rect, entity.motion, typeof(Motion), true);

	        if (entity.isNotCollapsed) {
		        if (entity.overrideInputIndex == -1) {
			        rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
			        _rect = new Rect(rect);
			        entity.Name = EditorGUI.TextField(_rect, "Name", entity.name);
		        }
	        }

        }

        private void ONLayerAddCallback(ReorderableList list) {
	        AnimatorLayer layer = new AnimatorLayer(this);
	        if (inputs.Count > 0) {
		        layer.stateMachine.SetStatesFromInputOptions(inputs[0].options);
	        }
	        layers.Add(layer);
        }
        
        // ####################### other ######################### //

		public string[] GetInputNames() {
			return inputs.Select(input => input.Name).ToArray();
		}

		public void Reset() {
			Clear();
			LoadAnimator(referenceAnimator);
		}

		public void Clear() {
			inputs.Clear();
			layers.Clear();
		}

		public void DrawSettings() {
			savePathCat = (AnimationSavePathCat)EditorGUILayout.EnumPopup(savePathCat);
			if (savePathCat == AnimationSavePathCat.Custom) {
				customSavePath = EditorGUILayout.TextField(customSavePath);
				if (GUILayout.Button("Select")) {
					customSavePath = EditorUtility.OpenFolderPanel("Select path for new Animation Clips", "", "Animations");
				}
			}
		}

		public Input FindInputByName(string name) {
			foreach (var input in inputs) {
				if (input.Name.Equals(name)) {
					return input;
				}
			}
			return null;
		}
        
		// ####################### load ######################### //
		
		public void LoadAnimator(AnimatorController anim) {
			referenceAnimator = anim;
			foreach (var parameter in referenceAnimator.parameters) {
				var input = new Input(this);
				input.parameterName = parameter.name;
				input.type = parameter.type;
				input.defaultFloat = parameter.defaultFloat;
				input.defaultInt = parameter.defaultInt;
				input.defaultBool = parameter.defaultBool;
				inputs.Add(input);
			}
			foreach (var layer in referenceAnimator.layers) {
				var layer2 = new AnimatorLayer(this);
				layer2.Name = layer.name;
				layer2.controllerLayer = layer;
				layers.Add(layer2);
				layer2.stateMachine = Load_AddStatesToStateMachine(layer.stateMachine);
			}
		}

		private StateMachine Load_AddStatesToStateMachine(AnimatorStateMachine stateMachine) {
			if (stateMachine == null) return null;
			StateMachine returnStateMachine = new StateMachine(this);
			returnStateMachine.Name = stateMachine.name;
			returnStateMachine.controllerStateMachine = stateMachine;
			foreach (var state in stateMachine.states) {
				var state2 = new StateProxy(this, returnStateMachine);
				state2.state.Name = state.state.name;
				state2.state.motion = state.state.motion;
				returnStateMachine.states.Add(state2);
			}
			foreach (var stateMachine2 in stateMachine.stateMachines) {
				var state2 = new StateProxy(StateType.StateMachine, this, returnStateMachine);
				Load_AddStatesToStateMachine(stateMachine2.stateMachine);
				returnStateMachine.states.Add(state2);
			}

			return returnStateMachine;
		}
		
		// ####################### save ######################### //
		
		public void Save() {
			//Save Data into Animator

			// parameters
			List<AnimatorControllerParameter> newParameters = new List<AnimatorControllerParameter>();
			foreach (var input in inputs) {
				AnimatorControllerParameter controllerParameter = new AnimatorControllerParameter();
				controllerParameter.name = input.parameterName;
				controllerParameter.type = input.type;
				controllerParameter.defaultBool = input.defaultBool;
				if (input.options.Count > input.defaultOptionIndex) {
					controllerParameter.defaultFloat = input.options[input.defaultOptionIndex].floatValue;
					controllerParameter.defaultInt = input.options[input.defaultOptionIndex].intValue; 
				}
				newParameters.Add(controllerParameter);
			}
			
			// layers
			List<AnimatorControllerLayer> newLayers = new List<AnimatorControllerLayer>();
			foreach (var layer in layers) {
				AnimatorControllerLayer controllerLayer = new AnimatorControllerLayer();
				controllerLayer.name = layer.Name;
				controllerLayer.stateMachine = AddStateMachine(layer.stateMachine);
				controllerLayer.defaultWeight = 1;
				newLayers.Add(controllerLayer);
			}

			referenceAnimator.parameters = newParameters.ToArray();
			referenceAnimator.layers = newLayers.ToArray();
		}

		private AnimatorStateMachine AddStateMachine(StateMachine stateMachine) {
			AnimatorStateMachine controllerStateMachine;
			
			if (stateMachine.overrideExistingStateMachine) {
				List<ChildAnimatorState> newStates = new List<ChildAnimatorState>();
				List<ChildAnimatorStateMachine> newStateMachines = new List<ChildAnimatorStateMachine>();
				controllerStateMachine = stateMachine.controllerStateMachine != null
					? stateMachine.controllerStateMachine
					: new AnimatorStateMachine();
				stateMachine.controllerStateMachine = controllerStateMachine;
				controllerStateMachine.name = stateMachine.Name;
				foreach (var state in stateMachine.states) {
					switch (state.type) {
						case StateType.State: {
							var temp = new ChildAnimatorState();
							temp.state = new UnityEditor.Animations.AnimatorState();
							temp.state.motion = state.state.motion;
							temp.state.name = state.state.Name;
							newStates.Add(temp);
							break;
						}
						case StateType.StateMachine:
						default: {
							var temp = new ChildAnimatorStateMachine();
							temp.stateMachine = AddStateMachine(state.stateMachine);
							newStateMachines.Add(temp);
							break;
						}
					}
				}

				controllerStateMachine.stateMachines = newStateMachines.ToArray();
				controllerStateMachine.states = newStates.ToArray();
			} else {
				controllerStateMachine = stateMachine.controllerStateMachine;
			}

			return controllerStateMachine;
		}

		// ##################### VRC SDK3 ####################### //

		
#if VRC_SDK_VRCSDK3
		public VRCExpressionsMenu mainMenu;
		
		public int CalcTotalCost()
		{
			int num = 0;
			foreach (Input parameter in this.inputs)
				num += TypeCost(parameter.type);
			return num;
		}

		public static int TypeCost(AnimatorControllerParameterType type) => type == AnimatorControllerParameterType.Int || type != AnimatorControllerParameterType.Bool ? 8 : 1;
		
		public void InitExpressionParameters(bool populateWithDefault) {
			var expressionParameters = inputs;
			if (populateWithDefault) {
				expressionParameters.Clear();

				Input input1 = new Input(this);
				input1.Name = "VRCEmote";
				input1.type = AnimatorControllerParameterType.Int;

				Input input2 = new Input(this);
				input2.Name = "VRCFaceBlendH";
				input2.type = AnimatorControllerParameterType.Float;

				Input input3 = new Input(this);
				input3.Name = "VRCFaceBlendV";
				input3.type = AnimatorControllerParameterType.Float;
				
				expressionParameters.Add(input1);
				expressionParameters.Add(input2);
				expressionParameters.Add(input3);
			} else {
				//Empty
				expressionParameters.Clear();
			}
		}
        
        // ############### VRC Expression Menus ################ //
        
        public void DrawVRCMainMenu() {
	        if (menuWrapper == null) {
		        menuWrapper = new VRCExpressionsMenuWrapper(new SerializedObject(mainMenu), this);
	        }

	        menuWrapper.MenuList.DoLayoutList();
        }
        
        private float ExpressionMenuElementHeightCallback(int index) {
            var entity = layers[index];

            float height = EditorGUIUtility.singleLineHeight * 1.25f;
            
            if (entity.stateMachine.isNotCollapsed) {
                height += EditorGUIUtility.singleLineHeight * 1.25f;
                height += EditorGUIUtility.singleLineHeight * 1.25f;
                height += entity.stateMachine.StatesRList.GetHeight();
                
                height += EditorGUIUtility.singleLineHeight * 0.5f;
            }

            return height;
        }

        private void DrawExpressionMenuElementCallback(Rect rect, int index, bool isactive, bool isfocused) {
            var entity = mainMenu.controls[index];
            
        }
#endif
	}

	public enum AnimationSavePathCat {
		InAnimatorManager,	// AM_Window.Instance.settingsAsset.AnimationsPath;
		InAnimatorFolder,	// AssetDatabase.GetAssetPath(referenceAnimator);
		Custom				// customSavePath
	}
}