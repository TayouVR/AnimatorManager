using System;
using System.Collections.Generic;
using System.Security.Principal;
using AnimatorManager.Scripts.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class NodeEditor: EditorWindow {
	
	List<Node> nodes = new List<Node>();
	List<int> windowsToAttach = new List<int>();
	List<int> attachedWindows = new List<int>();

	[MenuItem("Window/Node Editor")]
	static void ShowEditor() {
		NodeEditor editor = EditorWindow.GetWindow<NodeEditor>();

		editor.Init();
	}
   
	public void Init() {
		Rect startNode = new Rect(10, 10, 200, 50);  
		Rect endNode = new Rect(210, 210, 200, 50);
		Rect anyState = new Rect(320, 210, 200, 50);
		
		nodes.Add(new Node() {id = 0, name = "Start", rect = startNode, type = NodeType.Start});
		nodes.Add(new Node() {id = 0, name = "Any State", rect = anyState, type = NodeType.AnyState});
		nodes.Add(new Node() {id = 0, name = "End Node", rect = endNode, type = NodeType.End});
		windowsToAttach.Add(1);
		windowsToAttach.Add(2);
		
	}
 
 
	void OnGUI() {

		if (windowsToAttach.Count == 2) {
			attachedWindows.Add(windowsToAttach[0]);
			attachedWindows.Add(windowsToAttach[1]);
			windowsToAttach = new List<int>();
		}

		foreach (var node in nodes) {
			foreach (var transition in node.transitions) {
				foreach (var node2 in nodes) {
					if (node2.id == transition.endID) {
						DrawNodeCurve(node.rect, node2.rect);
					}
				}
			}
		}
 
		BeginWindows();
 
		if (GUILayout.Button("Create Node")) {
			nodes.Add(new Node() {rect = new Rect(10, 10, 100, 100), name = "State " + nodes.Count} );
		}
 
		for (int i = 0; i < nodes.Count; i++) {
			Node node = nodes[i];
			GUIStyle style = GUI.skin.window;
			switch (node.type) {
				case NodeType.Regular:
					break;
				case NodeType.Start:
					style = Styles.StartNode;
					break;
				case NodeType.End:
					style = Styles.EndNode;
					break;
				case NodeType.AnyState:
					style = Styles.AnyStateNode;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			node.rect = GUI.Window(i, node.rect, DrawNodeWindow, node.name, style);
		}
 
		EndWindows();
	}
 
 
	void DrawNodeWindow(int id) {
		if (GUILayout.Button("Attach")) {
			windowsToAttach.Add(id);
		}
 
		GUI.DragWindow();
	}

	void DrawNodeCurve(Rect start, Rect end) {
		Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
		Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
		Vector3 startTan = startPos + Vector3.right * 50;
		Vector3 endTan = endPos + Vector3.left * 50;
		Color shadowCol = new Color(0, 0, 0, 0.06f);
		for (int i = 0; i < 3; i++) // Draw a shadow
			Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
		Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.white, null, 3);
	}
}

public class Node {
	public NodeType type;
	public string name;
	public Rect rect;
	public int id;
	public List<Transition> transitions = new List<Transition>();

	public void AddTransition(int endID) {
		transitions.Add(new Transition() {id = transitions.Count, endID = endID, startID = id});
	}

	public void RemoveTransition(int transitionID) {
		foreach (var transition in transitions) {
			if (transition.id == transitionID) {
				transitions.Remove(transition);
			}
		}
	}
}

public enum NodeType {
	Regular,
	Start,
	End,
	AnyState
}

public class Transition {
	public int id;
	public int startID;
	public int endID;
}