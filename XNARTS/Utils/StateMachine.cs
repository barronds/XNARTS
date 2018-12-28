using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	// three kinds of interactions for clients:
	// 1) create state machine layout:
	//		- ask the factory in XStateMachineLayoutCollection to create you a 
	//		  XStateMachineLayout object that the client will fill out and register
	//		  in the collection under a string name.
	// 2) instance a state machine:
	//		- ask XStateMachineLayoutCollection to instance a state machine for a
	//		  given context object (optional).  
	// 3) use a state machine:
	//		- update the state machine and provide it input so that it might perform
	//		  transitions. (and get your callbacks called)

	// eg.	for a specific AI bot, say 'Dumb Guard', create a state machine layout
	//		that all the Dumb Guards will share.  fill it out.  later, whenever a 
	//		Dumb Guard is instantiated, it has an XStateMachine object within it
	//		that is created from the 'Dumb Guard' layout.  it will hold the state for 
	//		that instance of the state machine and the context (Dumb Guard Class?)
	//		and a reference to the layout.  inputs are provided to the state machine
	//		and the layout will decide if there is a transition or not and call the 
	//		callback with the context in the state machine.
	//		the design is this way so that state machine objects are small.  the layouts
	//		are much bigger and shared, saving a lot of memory.
	//		the callbacks are static methods and the parameter list will include the 
	//		context which should be an instance of that class, so that the static 
	//		method can act as a class method on an object.  that is the purpose of the 
	//		context.  it is optional.

	public class XStateMachine
	{
		// holds the state for one instance of a state machine.
		// holds the context object associated with the callbacks.
	}

	public class XStateMachineLayout
	{
		// holds the transition graph and callbacks for transitions and updates  
		// for one kind of state machine.  ie, one instance of this class for all
		// instances of state machines of that type.
	}

	public class XStateMachineLayoutCollection
	{
		// place where state machine layouts live, in a map, indexed on string.
		// has a factory that creates XStateMachine instances to go with a named (string) 
		// state machine layout.
	}
}
