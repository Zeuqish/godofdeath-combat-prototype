VAR item_name = "item_name"
VAR status_effect = "effect_none"
VAR name = "name"
VAR stat = "stat"
VAR effect_count = 0
VAR modifier = ""

=== prototype_done ===
You've finished the prototype!
-> END
=== enemy_action ===
Enemy used { name }!
-> END

=== enemy_do_nothing ===
The enemy is staring intently...
-> END

=== player_injured ===
You've been hit for a total of {effect_count} damage!
-> END

=== enemy_injured ===
The enemy has been hit for a total of {effect_count} damage!
-> END

=== player_healed ===
You've healed for a total of {effect_count} health!
-> END

=== enemy_healed ===
The enemy has healed for a total of {effect_count} health!
-> END

=== enemy_spirit_filled ===
Enemy Spirit Meter filled by {effect_count}!
-> END

=== player_stunned ===
Your turn has been skipped!
-> END

=== enemy_stunned ===
Enemy is stunned and their turn is skipped!
-> END

=== player_status_effect_applied ===
You are now {status_effect}!
-> END

=== player_status_effect_cleared ===
{status_effect} effect has worn off...
-> END

=== enemy_status_effect_applied ===
Enemy is now {status_effect}!
-> END

=== enemy_status_effect_cleared ===
Enemy stats are reverting to normal...
-> END

=== enemy_hostility_increased ===
{ 
  - effect_count > 15: 
        ~modifier = "significantly"
  - effect_count > 10:
        ~modifier = ""
  - effect_count > 0:
        ~modifier = "slightly"
}
Enemy hostility has increased {modifier}!
-> END

=== enemy_hostility_decreased ===

{ 
  - effect_count > 15: 
        ~modifier = "significantly"
  - effect_count > 10:
        ~modifier = ""
  - effect_count > 0:
        ~modifier = "slightly"
}
Enemy hostility has decreased {modifier}!
-> END

=== player_use_item ===
You used {item_name}!
-> END

=== enemy_increased_spirit_meter ===
Enemy Spirit Meter has increased by { effect_count }
-> END


=== passive_activate_origin_one ===
Passive Effect: Consecration - Ceremonial Variant Activated! 20 more Spirit Meter filled.
-> END

=== passive_activate_link_damage ===
Passive Effect: Puppet Piercing: Indiscriminate Jabs Activated! Spirit Meter filled by raw pre-status effect damage.
-> END

=== passive_activate_calling_one ===
"Passive Effect: Puppet Piercing: Heartseeking Acupuncture Activated! 10 more Spirit Meter filled.
-> END

=== passive_activate_calling_two ===
"Passive Effect: Puppet Piercing: Practiced Puncturing Activated! Enemy gains double damage and Spirit Meter Fill.
-> END

=== passive_activate_ordeal_two ===
Passive Effect: Standard Protocol Binding Spell Activated! All Spirit Meter fill negated.
-> END

=== passive_activate_journey_one ===
Passive Effect: Methodical Sigil Branding Activated! 10 more Spirit Meter filled.
-> END

=== passive_activate_journey_three ===
Passive Effect: Methodical Sigil Branding Activated! 10 more Spirit Meter filled.
-> END
