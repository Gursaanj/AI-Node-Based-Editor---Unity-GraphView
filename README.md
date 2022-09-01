# Behaviour Tree Editor Unity
 A node based behaviour tree editor following a [Hierarchical State Machine](https://www.eventhelix.com/design-patterns/hierarchical-state-machine/). Utilising Unity's experimental GraphView class, this editor is aimed at helping designer create and modify NPC AI logic without needing to open or change any coding behaviour. The tree is saved as a Scriptable Object (pure data structure) that can be called via the NPC's update loop. 
 
Inspired by [KiwiCoder's AI Behaviour Tree](https://www.google.com/url?sa=t&rct=j&q=&esrc=s&source=web&cd=&cad=rja&uact=8&ved=2ahUKEwiK0oL5s_P5AhVXHjQIHQ1fBQAQFnoECAUQAQ&url=https%3A%2F%2Fwww.gamedeveloper.com%2Fprogramming%2Fbehavior-trees-for-ai-how-they-work&usg=AOvVaw177VCUupApo8ZN6Jvox0qe).
 
![Behaviour Tree](https://user-images.githubusercontent.com/23490604/187896501-c57ace61-bc40-4e80-a8f0-f794127c6359.png)
An Example Behavior tree creating simple logic for an NPC to move towards a player and then attack it when witihin a given range

The editor has a grphview, which will allow users to add/delete and modify behaviour nodes (as well as other elements needed for better visualizing the tree), and Inspector view (on the left hand side) to allow users to inject information into specifci nodes when selected, a Blackoard as well as a minimap

Each Node when called attempts to complete it associated action, after completing said action it returns with the state SUCCESS or FAILURE.

There are 4 types of nodes (at time of writing) that help create the behaviour Structure

## Decorator Nodes
A decorator can only have one child. It decides when and how many times it child node is called. The base node in the project is the <b>Repeat Node</b>, which indicates how many time its child node gets called (-1 indicates and infinite loop). Only when child completes it action with a successful state, does the decorator node call the next iteration

![Decorator Node](https://user-images.githubusercontent.com/23490604/187896780-cf7601e2-09b6-4032-9eda-ef9de63ed6ad.png)

## Composite Nodes
A composite node can have unlimited children. It holds a sequence of child nodes that it will call in a given order (order of operation). Only when its child gives a successful state (after it completes its action), will the composite node call the next node's action. Only after successfully completing all its  child nodes does it return with a success state. If any of its child returns with a failure state, it will cease operation and too will return a failure state

![Composite Node](https://user-images.githubusercontent.com/23490604/187896822-70f0dc62-bced-4646-bc0f-ed404624bac6.png)
## Action Node
The leaf of the tree, the action node represents a single action the NPC will try to perform. If the action was able to be conducted correctly then it will return a successful state. Action nodes have no children, only parents. 

![Action Node 1](https://user-images.githubusercontent.com/23490604/187896870-88ea4849-47bf-433f-ad3f-58f408b4b120.png) ![Action Node 2](https://user-images.githubusercontent.com/23490604/187896885-cefaa836-bd25-4286-96e3-4ad72741cd8e.png)

## Root Node
The root of the tree. It is the starting point, which is called to initiate NPC's behaviour that frame. It can only have one child, no parent. There can only be one root node per tree. Each new tree starts with a root node.

![Root Node](https://user-images.githubusercontent.com/23490604/187897502-7f1c1ada-fcc5-4e57-b15d-78f7051e420e.png)


# Current Features

## Framing Support

Through context clicking the graph, it is possible to frame the view in such that all elements are presented at the same time (a current quality of life **todo** is to allow of framing for a custom selected amount of element)

![Frame All](https://user-images.githubusercontent.com/23490604/187899001-4ac9bd32-a45d-4d47-a2e5-e93e69a611f2.gif)


## Blackboard

you can add fields into a blackboard, which can be used to store global data and inject said information into certain nodes via Drag&Drop

![Blackboard elements](https://user-images.githubusercontent.com/23490604/187899193-148e9f44-ee64-4b2d-b519-6dd4a833fa15.png)

![DragAndDrop with BlackBoard](https://user-images.githubusercontent.com/23490604/187899207-836bd10c-389e-4348-9e57-ec7d5b952343.gif)
## Minimap

you can get a sense of the structure of the tree through a color-coordinated minimap. Selecting the outline of an element through the minimap will select the corresponding element in the graph.

![Minimap](https://user-images.githubusercontent.com/23490604/187899373-de9596aa-35f9-41cd-92f3-699b9024ebf1.png)

## StickyNotes

Sticky notes can be added anywhere to add needed comments/context and Todos (No Undo/Redo Support when adding this at the moment). Optimal for communicating/reasoning complex logic loops

![StickyNote](https://user-images.githubusercontent.com/23490604/187899531-082be179-3f46-450d-8abe-4b1c8fdcbf6c.png)

## Grouping

For better visibility of which node belongs to which feature (eg. Attack, Movement, Defending) you can group nodes,

![Create Group](https://user-images.githubusercontent.com/23490604/187899805-814d7304-a6bc-4f87-a451-e6f2c00a9cc3.gif)


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
