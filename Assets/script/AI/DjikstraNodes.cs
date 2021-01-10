using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DjikstraNodes{
	public Node  node;
	public int weight;
	public bool visited;
	public DjikstraNodes prevNode;

	//public DjikstraNodes leftNode;
	//public DjikstraNodes rightNode;
	//public DjikstraNodes upNode;
	//public DjikstraNodes downNode;

	public DjikstraNodes (){
	}

	public DjikstraNodes (Node _node, int _weight){
		node = _node;
		weight = _weight;
		prevNode = null;
		visited = false;

		//leftNode = null;
		//rightNode = null;
		//upNode = null;
		//downNode = null;

	}

	public int getweight(){
		return this.weight;
	}
}

