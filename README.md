# Behaviour Tree Editor Unity
 A node based behaviour tree editor following a [Hierarchical State Machine](https://www.eventhelix.com/design-patterns/hierarchical-state-machine/). Utilising Unity's experimental GraphView class, this editor is aimed at helping designer create and modify NPC AI logic without needing to open or change any coding behaviour. The tree is saved as a Scriptable Object (pure data structure) that can be called via the NPC's update loop. 
 
[Add Example Image 1 here]

Each Node when called attempts to complete it associated action, after completing said action it returns with the state SUCCESS or FAILURE.

There are 4 types of nodes (at time of writing) that help create the behaviour Structure

## Decorator Nodes
A decorator can only have one child. It decides when and how many times it child node is called. The base node in the project is the <b>Repeat Node</b>, which indicates how many time its child node gets called (-1 indicates and infinite loop). Only when child completes it action with a successful state, does the decorator node call the next iteration

[Decorator Node Image]

## Composite Nodes
A composite node can have unlimited children. It holds a sequence of child nodes that it will call in a given order (order of operation). Only when its child gives a successful state (after it completes its action), will the composite node call the next node's action. Only after successfully completing all its  child nodes does it return with a success state. If any of its child returns with a failure state, it will cease operation and too will return a failure state

[Composite Node Image]

## Action Node
The leaf of the tree, the action node represents a single action the NPC will try to perform. If the action was able to be conducted correctly then it will return a successful state. Action nodes have no children, only parents. 

[Action Node Image]

## Root Node
The root of the tree. It is the starting point, which is called to initiate NPC's behaviour that frame. It can only have one child, no parent. There can only be one root node per tree. Each new tree starts with a root node.


[Root Node Image]

# Current Features

## Framing Support


## Blackboard

you can add fields into the blackboard and inject the information into certain nodes via Drag&Drop

[Add Gif Here]

## Minimap

you can get a sense of the structure of the tree through a color-coordinated map

[Add Image]

## StickNotes

Sticky notes can be added anywhere to add needed comments/Todo (No Undo/Redo Support when adding this at the moment)

[Add Image]

## Grouping

For better visibility of which node belongs to which feature (eg. Attack, Movement, Defending) you can group nodes

[Add Gif]


# TODO/ UPCOMING CHANGES

- Fixed Undo/Redo Support for all features (not just moving/adding/removing of nodes)
- Fixed Grouping support on multiple elements
- Fixed Runtime Clonging (currently errors will occur if trying to start your Unity Application whilst trying to clone the behaviour tree in order to visualize which nodes are currently being hit)
- Add Manual Save/Load features
 - Currently changes made are permanently added onto the behaviour tree data structure, instead of when just being saved.
 - Also just by clicking the Behaviour Tree SO the editor will wipe out any exisitng data and replacce it with the tree that was just clicked. This should change to a double click instead to ensure no loss of information
 - Need to create an EditorDisplayDialogue when the behaviour tree becomes "dirtied" with modified information
 - Add copy/cut/paste support for nodes
 
 
 The rest are just Quality of life improvements that will be addressed in the near future
