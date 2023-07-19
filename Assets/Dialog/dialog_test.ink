VAR is_finished = false

{is_finished:
-> reminder
-else: -> intro
}


== intro ==
Do you want to leave this place?
It's simple. Do you want to learn more?
    * [Yeah.] -> choice_1
    * [Maybe later.]  -> choice_2
    
=== choice_1 ===
 -Ah, well, just interact with that entity there and you can go your merry way.
 ~is_finished = true
 
 -> END

=== choice_2 ===
-Oh, well, just come back if you want to learn more.
 -> END

=== reminder ===
-Remember just interact with that entity over there to leave.
-> END

=== area_not_open ===
 -It appears I can't access this area yet. I feel like I need to do something first. Maybe I should interact with the place?
 -> END