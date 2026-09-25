# Game Design Document: Flappy Bird (Unity Remake)

| Field | Value |
| --- | --- |
| Working title | Flappy Bird: Unity Remake |
| Genre | Arcade / endless side-scroller |
| Platform | PC (Windows, macOS, Linux) with Unity |
| Engine | Unity `6000.3.20f1`, Universal Render Pipeline (2D) |
| Orientation | Landscape, one fixed screen |
| Players | Single player |
| Session length | About 10 seconds to a few minutes per run |
| Target audience | All ages, casual players who like chasing high scores |
| Art style | Pixel art made by code, 32 pixels per unit |

## 1. Overview

### 1.1 High concept

This is a simple endless arcade game with one button. The player keeps a bird in the air by tapping, and tries to fly it through gaps between pipes. If the bird touches anything, the run ends. The idea is that the rules are easy to understand, and after you crash you can restart right away and try again.

### 1.2 Design pillars

1. **One input.** The player only chooses when to tap. Nothing else is controlled by them, so the game feels fair and clear.
2. **Fast restart.** Losing only costs a few seconds. You can start again quickly, so it does not feel too frustrating.
3. **Clear feedback.** When you score, hit a milestone, or crash, the game shows and plays something clear so you always know what happened.
4. **Everything made in the project.** The art and sounds are created by an editor script, so we do not need outside asset packs.

### 1.3 Player experience goals

The player should feel challenged, but not cheated. Early in a run the pace is steady so you can learn the timing. From score 15 the game gets clearly harder with a big speed boost, so high scores take more skill. Messages like NICE / LEGENDARY and medals help make progress feel more rewarding than just watching a number go up.

## 2. Core gameplay

### 2.1 The loop

```
Ready screen  ->  choose bird colour  ->  tap to start
      ^                                       |
      |                              Playing: flap, dodge, score
      |                                       v
  Restart  <-  Game Over: feathers, score, best, medal
```

### 2.2 Controls

| Action | Input | Notes |
| --- | --- | --- |
| Start run | Left click / `Space` / `Up Arrow` | Same input as flying up |
| Fly upward | Left click / `Space` / `Up Arrow` | Sets a fixed upward speed |
| Restart | Click **RESTART** | Only works on the game over screen |

Clicks on UI (like the colour buttons) do not start the game by mistake.

### 2.3 Bird physics

The bird stays at a fixed X position. When you flap, its upward speed is set to a fixed value (not added on top of the old speed). That way every tap feels the same.

| Property | Value | Purpose |
| --- | --- | --- |
| Flap velocity | `5.6` | How fast the bird goes up after a tap |
| Gravity scale | `2.7` | How fast it falls |
| Max fall speed | `-10` | Top falling speed |
| Rotate up angle | `28°` | Bird tilts up after a flap |
| Rotate down angle | `-90°` | Bird tilts down while falling |
| Rotate lerp | `8` | How smooth the tilt change is |
| Flap animation | `10` fps | Three wing frames |
| Idle bob | `0.18` amplitude, `3.4` speed | Small bounce on the ready screen |

The hitbox is a circle that is a bit smaller than the bird sprite, so close calls feel fair.

### 2.4 World and obstacles

| Property | Value | Purpose |
| --- | --- | --- |
| Scroll speed | `2.55` units/sec | Base world speed (left) |
| Hard mode from score | `15` | When the late-game challenge starts |
| Hard mode scroll multiplier | `1.5` | World becomes 50% faster from that score |
| Pipe spawn interval | `1.35` sec | Time between new pipe pairs |
| Pipe gap | `2.25` units | Size of the opening the bird must fly through |
| Gap centre range | `-1.15` to `2.05` | Random height of each gap |
| Spawn X | `7.5` | Pipes appear off the right side of the screen |
| Despawn X | `-8.5` | Pipes leave off the left side and go back to the pool |
| Ground Y | `-4.7` | Death line at the bottom of the screen |
| Ceiling Y | `4.85` | Death line at the top of the screen |

Pipe pairs are reused from a pool instead of being created and deleted every time. That keeps the game smoother.

The ground and ceiling death areas are placed at the edges of the screen you can see. In an older version the ground kill zone was too high, so the bird died in the middle of the screen. Now it only dies when it reaches the real bottom.

### 2.5 Scoring

You get 1 point when the bird passes the middle of a pipe pair. Each pair gives a point only once. The score is shown as a big number at the top. The best score is saved with `PlayerPrefs` (key: `FlappyBird.BestScore`) and updates when you beat your old record.

### 2.6 Fail state

Touching anything tagged `Hazard` (pipes, ground, ceiling, dragon fireballs) ends the run. Then the game:

1. Changes state to `GameOver`.
2. Plays the feather burst at the bird.
3. Stops spawning and scrolling pipes.
4. Turns off the dragon and the hard-mode rain.
5. Lets the bird fall with stronger gravity.
6. Plays the hit sound.
7. Shows the game over panel after `0.85` seconds.

The short wait before the panel lets you see the crash and the feathers.

## 3. Features

### 3.1 Bird colour selection

On the ready screen there are six colours: yellow, red, orange, green, light blue and purple. The selected one has a bright white border. The others look more faded.

This is only cosmetic. All birds have the same size, hitbox, flap strength and gravity. The choice does not make the game easier or harder. The selected colour is saved in `PlayerPrefs` for next time.

### 3.2 Milestone cheers

Every 5 points, a short message appears above the bird. It grows, moves up a bit, and fades out in about `1.15` seconds. It has an outline so it stays readable over the pipes.

| Score | Message | Colour |
| --- | --- | --- |
| 5 | NICE | Warm yellow |
| 10 | WOW | Warm yellow |
| 15 | EPIC | Warm yellow |
| 20 | LEGENDARY | Gold |
| 25 | UNSTOPPABLE | Cyan |
| 30 | GODLIKE | Green |
| 40+ | MYTHIC | Violet |

Higher scores get stronger words and different colours. This is meant to encourage the player.

### 3.3 Feather burst

When the bird hits a pipe or the ground, 12 small feathers come out from that point. Each feather has a random direction, spin, size and lifetime. They fall down, move a bit sideways, and fade out.

The feathers use the same colour as the bird you chose. They are also reused from a pool, like the pipes.

### 3.4 Fireworks and celebration audio

From score 10 and up, each milestone also plays fireworks around the bird and a short celebration sound.

### 3.5 Hard mode (speed + rain)

From score **15**, the game raises the challenge on purpose so late runs do not feel too easy.

- **Faster scroll.** The world speed becomes `scrollSpeed * hardModeScrollMultiplier` (`2.55 * 1.5`). Pipes, ground and background all use this shared speed so everything stays in sync.
- **Rain decoration.** Soft blue raindrops fall from the top of the screen for the rest of the run. They are visual only. No collider, no damage, no effect on physics. Rain starts at the same score as the speed boost and stops on crash or restart.

Together, the speed jump is the real challenge, and the rain is a cute cue that hard mode has started.

### 3.6 Dragon hazard

At score 15, a dragon comes in from the right. It slowly follows the bird's height and shoots fireballs sideways. Because it follows slowly and only shoots left/right, you can still dodge by flying higher or lower.

The dragon only shows up if its art exists. If the art is missing, it does not activate, so you will not die from an invisible fireball.

### 3.7 Medals

| Score | Medal |
| --- | --- |
| 10 to 19 | Bronze |
| 20 to 29 | Silver |
| 30 to 39 | Gold |
| 40+ | Platinum |

Medals are shown on the game over screen for that run.

## 4. Systems architecture

### 4.1 State machine

`GameManager` is a singleton and controls the main game state:

| State | What happens |
| --- | --- |
| `Ready` | Bird bobs, colour picker is shown, world is still, waiting for input |
| `Playing` | Gravity is on, pipes spawn and move, scoring works |
| `GameOver` | Scrolling stops, bird falls, game over panel shows after a delay |

### 4.2 Script responsibilities

| Script | Responsibility |
| --- | --- |
| `GameManager` | Game states, score, milestones, restart, startup fixes |
| `GameState` | State enum |
| `GameConfig` | ScriptableObject with all tuning numbers |
| `BirdController` | Gravity, flap, tilt, animation, collision |
| `FlapInput` | Reads mouse and keyboard input |
| `PipeSpawner` | Pipe pool and spawn timing |
| `PipePair` | Places each pair, moves it, gives the score |
| `ScrollRepeater` | Loops the background and ground |
| `GameUI` | HUD, ready/game over screens, cheers, medals |
| `BirdSelect` | Six colour options and saving the choice |
| `GameEffects` | Sparks, feathers, fireworks, hard-mode rain |
| `Dragon`, `Fireball` | Late game hazard |
| `GameAudio` | Flap, point, hit and death sounds |
| `SpriteLibrary` | Loads sprites, materials and audio at runtime |

### 4.3 Data-driven tuning

All the numbers from section 2 are in `Assets/Data/GameConfig.asset`. You can change difficulty in the Inspector without editing code.

### 4.4 Robustness decisions

Some safety choices help the game work the same after cloning from GitHub:

- Scene sprite materials are fixed at runtime if needed, so objects do not stay invisible.
- The bird loads its sprite from `Resources`, and can draw a backup sprite in code if needed.
- Pipes can also use a backup sprite drawn in code, instead of invisible deadly pipes.
- The dragon does not start without its art.
- Ground and ceiling death areas are set from the config at startup so they match the screen.

## 5. Art and audio

### 5.1 Art

All sprites are made by `Assets/Editor/GenerateFlappyBirdArt.cs` at 32 pixels per unit, with point filtering and no compression.

| Asset | Size (px) | Notes |
| --- | --- | --- |
| Bird | 34 x 24 | Three flap frames, transparent background, no black frame |
| Pipe | 52 x 320 | Body and lip |
| Background | 576 x 384 | Sky with clouds and hills |
| Ground | 336 x 112 | Scrolling ground strip |
| Medals | 48 x 48 | Four medal types |
| Panel | 220 x 140 | Game over panel |
| Dragon | 64 x 48 | Late game enemy |
| Fireball | 20 x 20 | Dragon shot |
| Spark | 10 x 10 | Used for fireworks / particles |

The camera background colour is cyan (`78, 192, 202`), and orthographic size is `5`.

### 5.2 Audio

Sounds are also generated as WAV files by the same editor script:

| Clip | What it is | When it plays |
| --- | --- | --- |
| `flap` | Short 780 Hz tone | Every flap |
| `point` | Two tones 980 to 1320 Hz | Passing a pipe pair |
| `hit` | Noise burst | Collision |
| `die` | Falling tone 420 to 110 Hz | Hitting the ground |
| `wow` | Short arpeggio | Every 5 points |
| `firework` | Burst sound | Fireworks from score 10 |

## 6. UI

| Screen | Elements |
| --- | --- |
| Ready | Best score, `TAP / SPACE`, bobbing bird, **CHOOSE BIRD** |
| Playing | Big score at the top, short cheer messages |
| Game Over | `GAME OVER`, final score, best score, medal, **RESTART** |

During play the HUD stays simple. Mostly only the score is always on screen, so it does not block the pipes.

## 7. Scope and future work

### 7.1 What is already in the game

- Full ready -> playing -> game over loop with saved best score
- Endless random pipes with object pooling
- Six cosmetic bird colours
- Cheer messages every 5 points
- Feather burst on hit, matching the bird colour
- Hard mode from score 15: faster scroll plus decorative rain
- Fireworks, alternate bird look, and dragon as score rewards
- Four medal levels
- Art and audio generated by code

### 7.2 Possible future ideas

- Mobile version with touch controls
- Day / night backgrounds based on score
- Local leaderboard with names
- Options for fewer particles or easier gravity
- Extra bird skins unlocked by medals
