using Contracts; // base stock game's Contract namespace.
using ContractConfigurator; // extensions from the mod
using ContractConfigurator.Parameters; // extensions from the mod
using ContractConfigurator.Util; // extensions from the mod
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KOSContractPack
{
    // The various types of user controls that can be tested for.
    // These enum strings are meant to appear directly in the
    // Contract Configurator .cfg file when defining which controls
    // are being banned:
    public enum PlayerControlName
    {
        All, // pseudonym that just denies all of the following.
        Throttle,
        WheelThrottle,
        Pitch,
        PitchTrim,
        Roll,
        RollTrim,
        Yaw,
        YawTrim,
        TranslateX,
        TranslateY,
        TranslateZ
    }
    
    /// <summary>
    /// A Contract Configurator Parameter used to ensure that the
    /// player hasn't touched any of the manual controls mentioned
    /// in the data for this parameter.
    /// </summary>
    public class PlayerControlsBannedFactory : ParameterFactory
    {
        protected List<PlayerControlName> controls { get; set; }
        
        public override bool Load(ConfigNode node)
        {
            if (! base.Load(node))
                return false;
            
            // There is only one excess argument beyond the standard ones, the "controls":
            bool valid = ConfigNodeUtil.ParseValue<List<PlayerControlName>>(node, "control", x => controls = x, this);
            return valid;
        }
        
        public override ContractParameter Generate(Contract contract)
        {
            return new PlayerControlsBanned(this.title, controls);
        }
    }
    
    // The actual enforcement of the contract.
    public class PlayerControlsBanned : VesselParameter
    {
        protected List<PlayerControlName> controls { get; set; }
        
        // ContractConfigurator needs to do a Reflection walk in which it wants to
        // instance a dummy of this class, so it needs a default constructor or it
        // throws exceptions in the log:
        public PlayerControlsBanned()
        {
            controls = new List<PlayerControlName>();
        }

        public PlayerControlsBanned(string title, List<PlayerControlName> controls)
        {
            this.title = title;
            this.controls = controls;
            
            // This next line doesn't seem to be having any effect.
            // Not sure why.
            this.notes = BuildNotesString();
        }
        
        private string BuildNotesString()
        {
            StringBuilder buildNotes = new StringBuilder();
            buildNotes.Append("Player must leave ");
            foreach (PlayerControlName control in controls)
            {
                if (control != controls.FirstOrDefault())
                {
                    if (controls.Count > 2 )
                    {
                        buildNotes.Append(", ");
                    }
                    else
                    {
                        buildNotes.Append(" ");
                    }
                    
                    if (control == controls.LastOrDefault())
                    {
                        buildNotes.Append("and ");
                    }
                }
            }
            buildNotes.Append(" manual control");
            if (controls.Count > 1)
            {
                buildNotes.Append("s");
            }
            buildNotes.Append(" at the zero position.");
            
            return buildNotes.ToString();
        }

        protected override void OnParameterSave(ConfigNode node)
        {
            base.OnParameterSave(node);
            
            // ContractConfigurator's way of reading lists from a 
            // ConfigNode is a layer on top of the stock KSP's
            // ConfigNode system that isn't part of stock.  To
            // make the list re-readable during load by
            // ContractConfigurator, it has to be manually written
            // into the ConfigNode like so:
            //    listName = value1
            //    listName = value2
            //    listName = value3
            //  etc - use the same key name again for each value,
            // and ContractConfigurator will be able to detect that
            // that means they're all in the same List later when
            // parsing them back in.

            foreach (PlayerControlName control in controls)
            {
                node.AddValue("control", control.ToString());
            }
        }

        protected override void OnParameterLoad(ConfigNode node)
        {
            base.OnParameterLoad(node);

            controls = ConfigNodeUtil.ParseValue<List<PlayerControlName>>(node, "control", false);
        }
        protected override void OnRegister()
        {
            base.OnRegister();
        }

        protected override void OnUnregister()
        {
            base.OnUnregister();
        }
        protected override void OnPartAttach(GameEvents.HostTargetAction<Part, Part> e)
        {
            base.OnPartAttach(e);
            // CheckVessel is part of VesselParameter.  It is told which vessel to perform
            // this parameter check on.  We only use the active vessel here, despite kOS
            // being able to control multiple vessels, because only the active vessel can
            // receive player control inputs anyway:
            CheckVessel(FlightGlobals.ActiveVessel, false);
        }

        protected override void OnPartJointBreak(PartJoint p)
        {
            base.OnPartJointBreak(p);
            // CheckVessel is part of VesselParameter.  It is told which vessel to perform
            // this parameter check on.  We only use the active vessel here, despite kOS
            // being able to control multiple vessels, because only the active vessel can
            // receive player control inputs anyway:
            CheckVessel(FlightGlobals.ActiveVessel, false);
        }

        // This is a hook that is called by VesselParameter whenever it wants
        // to query the parameter state.  This is where the main logic of this
        // class is.  If the conditions are met, it returns true, if not, it
        // returns false.
        //
        // In this case the conditions are "is the player leaving the specified
        // controls in the banned list untouched?"  If so, return true, else false.
        //
        protected override bool VesselMeetsCondition(Vessel vessel)
        {
            
            // The logic here is designed to quit as soon as it finds any banned control is active, without bothering
            // to check if any of the other controls are active too.  Once you know it's going to be false, it can't
            // become even more false just because it failed even more conditions:
             
            foreach (PlayerControlName control in controls)
            {
                switch(control)
                {
                    case PlayerControlName.All:
                        if (! FlightInputHandler.state.isNeutral)
                            return false;
                        break;
                    case PlayerControlName.Throttle:
                        if (FlightInputHandler.state.mainThrottle > 0.0f)
                            return false;
                        break;
                    case PlayerControlName.WheelThrottle:
                        if (FlightInputHandler.state.wheelThrottle > 0.0f)
                            return false;
                        break;
                    case PlayerControlName.Pitch:
                        if (FlightInputHandler.state.pitch != 0.0f)
                            return false;
                        break;
                    case PlayerControlName.PitchTrim:
                        if (FlightInputHandler.state.pitchTrim != 0.0f)
                            return false;
                        break;
                    case PlayerControlName.Roll:
                        if (FlightInputHandler.state.roll != 0.0f)
                            return false;
                        break;
                    case PlayerControlName.RollTrim:
                        if (FlightInputHandler.state.rollTrim != 0.0f)
                            return false;
                        break;
                    case PlayerControlName.Yaw:
                        if (FlightInputHandler.state.yaw != 0.0f)
                            return false;
                        break;
                    case PlayerControlName.YawTrim:
                        if (FlightInputHandler.state.yawTrim != 0.0f)
                            return false;
                        break;
                    case PlayerControlName.TranslateX:
                        if (FlightInputHandler.state.X != 0.0f)
                            return false;
                        break;
                    case PlayerControlName.TranslateY:
                        if (FlightInputHandler.state.Y != 0.0f)
                            return false;
                        break;
                    case PlayerControlName.TranslateZ:
                        if (FlightInputHandler.state.Z != 0.0f)
                            return false;
                        break;
                    default:
                        break; // must mean someone mentioned a control name we don't test for.
                }
            }
            return true;
        }
        
        // The VesselParameter base class has a CheckVessel method that does all the regular
        // update work, but because not all of VesselParameter's derived classes need to waste the
        // CPU time on checking constantly, it doesn't fire off unless the derived class explicitly
        // tells it to in its update, as is done below.  (for example, a check to see if you docked
        // with something only needs to fire off when an dock event happens, rather than all the time).
        //
        // For our needs, we have to check all the time to make sure the player never touches
        // the controls:
        //
        // If this turns out to be slowing things down, it's always possible to throttle this back
        // a bit by only checking every fifth of a second or so - because user input is slow and that
        // would still catch it.
        //
        protected override void OnUpdate()
        {
            base.OnUpdate();
            CheckVessel(FlightGlobals.ActiveVessel);
        }
    }
}