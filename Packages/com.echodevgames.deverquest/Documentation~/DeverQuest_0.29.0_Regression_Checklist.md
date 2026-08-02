# DeverQuest 0.29.0 Regression Checklist

## Install and migrate

1. Install the `0.29.0` tarball through Unity Package Manager.
2. Confirm **Tools > DeverQuest** opens with no Console errors.
3. Confirm the existing Adventurer, Guild account, XP, total purse value,
   inventory, Companions, and Chronicle history remain intact.
4. Confirm the Character and Rewards views show physical coin and carry
   weight.

## Generate tactical content

1. Sign in as a Boss or CEO.
2. Open **Settings > Rules Laboratory**.
3. Choose **Generate Tactical Starter Kit + Quest Templates**.
4. Run it a second time and confirm it updates rather than duplicates assets.
5. Inspect `Assets/DeverQuest/Tactical` and confirm every Class Definition has
   its own editable Tactical Codex.

## Fifteen-minute skirmish

1. Offer and accept **Fifteen-Minute Skirmish**.
2. Before 15 focused minutes, choose
   **Report Development Objective Complete**.
3. Confirm the Chronicle records an early Stage completion and its bonus.
4. Confirm the Battle Chronicle separately reports Victory/Early Victory,
   rounds versus par, actions, typed damage, and spoils.
5. Repeat without early turn-in and confirm only normal Stage rewards apply.

## Tactical effects

1. Test a caster with the generated starter Spells.
2. Confirm healing is preferred below its configured HP threshold.
3. Confirm ongoing damage ticks for its duration.
4. Confirm root/snare/control changes enemy turns or attack rolls.
5. Confirm mana costs and cooldowns prevent impossible repeat casting.
6. Confirm a monster Ability Profile can apply an ongoing condition.

## Survival expedition

1. Accept **Wayfarer Survival Expedition**.
2. For testing, temporarily reduce its wave interval.
3. Confirm one wave resolves per interval and difficulty/rewards grow.
4. Confirm a failed flee check pauses the work timer.
5. Confirm Homeward Sigil exits for a caster that knows it.
6. Confirm the Guild wagon is available only at configured checkpoints.
7. Confirm turning in the work session first attempts a safe exit.

## Health and carry safety

1. Lower the Adventurer near the Encounter's safety threshold.
2. Confirm combat pauses at 1 HP or the configured threshold before another
   enemy turn, and the Encounter Danger audio cue plays.
3. Accumulate weighted scrap until the pack is over capacity.
4. Confirm the fight and work timer pause.
5. Drop items and recover above one-quarter HP, then resume.
6. Earn more than 100 copper, verify it remains loose coin, then use
   **Exchange Coin Denominations at Guild Hall**.
7. Confirm total purse value is unchanged while piece count and weight fall.

## Final Chronicle

1. Finalize the session.
2. Confirm the Markdown and JSON timecard include Stage pace, early flags,
   survival waves, tactical actions, par, safety reasons, loot, and carry
   weight.
3. Confirm older Battle entries still render without fabricated action data.
