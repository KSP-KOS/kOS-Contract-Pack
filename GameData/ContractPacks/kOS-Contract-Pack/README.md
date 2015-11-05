This directory is meant to contain the .cfg contract configurator files.

### Using Contract Extension Parameter PlayerControlsBanned

This contract pack has a small Plugin DLL containing a new 
extended contract parameter that is useful for making kOS
contracts: 

PlayerControlsBanned
====================

A PlayerControlsBanned parameter is considered satisfied whenever
the manual controls listed in its attributes are in their zeroed
(neutral) state, and considered failed if any of them are not
in their zeroed state.

For example, the following example is a PlayerControlsBanned
parameter that disallows the use of manual pitch, pitch trim,
and throttle, but still allows the use of anything else::

    PARAMETER
    {
        name = hands_off_buddy
        type = PlayerControlsBanned
        title = Leave throttle and pitch at zero

        // List multiple instances of 'control = ...' to ban
        // more than one control:
        control = Throttle
        control = Pitch
        control = PitchTrim
    }

Only player's manual controls are affected
------------------------------------------

Obviously the script is still allowed to activate the controls.  This
only checks the PILOT controls to ensure the script is doing the work
and not the player pilot.

This checks the equivalent of the SHIP:CONTROL:PILOTfoo suffixes.

Players should be warned about the annoying 50% throttle thing
--------------------------------------------------------------

Because SQUAD defaults the throttle to 50%, scripts that launch right
from bootup will auto-fail any attempt to ban the throttle because
the main game will set it to 50% by default hidden underneath the
controls the script is doing.  Therefore scripts will need to
probably ``SET SHIP:CONTROL:PILOTMAINTHROTTLE TO 0.`` in order not
to fail contracts that have ``control = Throttle`` or ``control = All``
in their conditions.  Players probably need to be hinted about this
in contract descriptions that try to ban the use of the throttle.

Full List of controls recognized
--------------------------------

This is the full list of all the keywords that are recognized by
PlayerControlsBanned as possible values to the ``control = ...``
attributes:

- ``All`` : a shorthand way of saying all the things below are banned, without having to list them one by one.
- ``Throttle`` : the player throttle must be at its zero position during flight.
- ``WheelThrottle`` : when grounded : You mustn't drive fore/back with the "W" and "S" keys.
- ``Pitch`` : The "W" and "S" keys in flight, or joystick equivalent axis, must be centered.
- ``PitchTrim`` : The pilot trim for pitch (i.e. alt-W and alt-S) must be centered.
- ``Roll``: The "Q" and "E" keys, or joystick equivalent axis, must be centered.
- ``RollTrim`` : The pilot trim for roll (i.e. alt-Q and alt-E) must be centered.
- ``Yaw``: The "A" and "D" keys, or joystick equivalent axis, must be centered.
- ``YawTrim`` : The pilot trim for yaw (i.e. alt-A and alt-D) must be centered.
- ``TranslateX`` : The "J" and "K" keys, or joystick equivalent axis, must be centered.
- ``TranslateY`` : The "I" and "J" keys, or joystick equivalent axis, must be centered.
- ``TranslateZ`` : The "H" and "N" keys, or joystick equivalent axis, must be centered.
