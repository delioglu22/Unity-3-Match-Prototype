# Match-3 Puzzle Game
 
A technical prototype developed in Unity to demonstrate 2D match-3 puzzle mechanics, state-based board management, swipe input handling, cascade scoring, and dynamic board refilling. The project focuses on classic match-3 gameplay where the player swipes adjacent pieces to form matches of three or more, scores points, and clears the board within a limited number of moves.
 
## 🎮 Key Features & Technical Details
 
### 1. State-Based Game Flow (BoardManager.cs)
The board operates on a strict state machine to prevent conflicting input or processing during animations.
* **Game States:** Three distinct states (`Move`, `Wait`, `GameOver`) ensure the player can only interact when the board is fully settled.
* **Move Limit:** The game enforces a configurable move counter (`maxMoves = 15`). Each valid swap (one that produces a match) consumes a move.
* **Game Over Detection:** When moves reach zero after a board processing cycle, the state transitions to `GameOver`.
* **Singleton Pattern:** `BoardManager` and `UIManager` both use singleton instances for global access across components.
### 2. Board Generation & Setup
* **No-Match Initialization:** The board is filled procedurally while actively preventing pre-existing matches. During placement, each cell checks its two left and two bottom neighbors before accepting a random piece type, guaranteeing a clean start.
* **Centered Layout:** Board coordinates are offset by `(width - 1) / 2f` and `(height - 1) / 2f` so the grid is always centered in world space regardless of dimensions.
* **Configurable Size:** `width` and `height` are serialized properties, allowing board dimensions to be adjusted directly from the Unity Inspector.
### 3. Swipe Input & Piece Movement (Piece.cs)
* **Mouse / Touch Swipe:** Each piece detects `OnMouseDown` and `OnMouseUp` events to capture swipe direction using vector subtraction between first and final touch positions.
* **Swipe Resistance:** A `swipeResist` threshold filters out accidental micro-swipes; only gestures exceeding this magnitude trigger a swap.
* **Angle-Based Direction:** The swipe angle (calculated via `Mathf.Atan2`) is mapped to one of four cardinal directions (up, down, left, right) using 45-degree quadrant checks.
* **Smooth Lerp Movement:** Pieces move to their `targetPosition` each frame using `Vector2.Lerp`, giving a fluid sliding animation without relying on physics or tweening libraries.
* **Swap & Rollback:** After a swap, if no match is produced, piece positions, board array references, and location metadata are all swapped back atomically using C# tuple deconstruction.
### 4. Match Detection (HasMatches)
* **Horizontal & Vertical Scanning:** The board is scanned row by row and column by column. Any run of three or more identical piece types triggers a match.
* **Extensible Runs:** After finding a base match of three, the scanner continues walking in the same direction to capture runs of four, five, or more pieces.
* **Match Flags:** Each matched piece receives `isMatched`, `isHorizontalMatch`, and/or `isVerticalMatch` flags, enabling bonus scoring for cross-shaped (T/L) matches.
### 5. Scoring System
* **Base Score:** Each destroyed piece awards `baseScoreValue × scoreMultiplier` points.
* **Cross-Match Bonus:** Pieces that are part of both a horizontal and a vertical match simultaneously receive an additional `50 × scoreMultiplier` bonus.
* **Cascade Multiplier:** If the board produces automatic chain matches after gravity and refill (without any player swap), `scoreMultiplier` increments for each cascading wave, rewarding skilled setups.
* **Multiplier Reset:** The multiplier resets to `1` at the start of every player-initiated swap to keep scoring fair.
* **Real-Time UI:** Score is pushed to `UIManager` after every destroy cycle.
### 6. Gravity & Board Refill
* **Column Gravity:** After destroying matched pieces, `ApplyGravity()` scans each column bottom-up, tracking a running `nullCount`. Pieces above empty cells slide down by exactly the number of gaps beneath them.
* **Spawn from Above:** `RefillBoard()` instantiates new pieces above the visible grid (offset by `+5f` on the Y axis) and assigns them a `targetPosition` at the correct cell, so they visually drop into place via the Lerp system.
* **Cascade Check:** After refilling, `HasMatches()` is called again. If new matches exist, `ProcessBoardRoutine()` recurses, triggering the cascade multiplier.
### 7. Piece Types
The `PieceType` enum defines seven variants: **Blue, Green, Red, Yellow, Purple, Pink, Box** — all driven by prefab assignment in the Inspector, keeping art and logic fully decoupled.
 
## 🕹️ Controls
 
* **Click + Drag (Mouse) / Swipe (Touch):** Swipe a piece Up, Down, Left, or Right to swap it with its neighbor.
* **Objective:** Form matches of 3 or more identical pieces to score points before running out of moves.
## 🎯 Gameplay Mechanics
 
### Matching & Clearing
- Matches of exactly 3 pieces award base score per piece.
- Longer matches (4+) award the same per-piece rate but contribute to larger cascades.
- Cross-shaped matches (a piece involved in both a horizontal and vertical match) award a bonus.
### Cascade System
- After every clear, gravity pulls remaining pieces down and new pieces fall from above.
- If these movements create new matches, they resolve automatically with an increasing score multiplier.
- Cascades continue until the board is stable.
### Move Economy
- Only swaps that result in at least one match consume a move.
- Invalid swaps (no match found) are reversed with no move penalty.
- The game ends when the move counter reaches zero after the board finishes processing.
## 🛠️ Installation & How to Run
 
1. Clone this repository:
   ```bash
   git clone <https://github.com/delioglu22/Unity-3-Match-Prototype.git>
   ```
2. Open **Unity Hub**.
3. Click **Add** and select the cloned folder.
4. Open the project (Recommended Version: Unity 2021.3 LTS or later).
5. Ensure **TextMeshPro** is installed via Package Manager.
6. Open the main scene and press **Play**.
## 📁 Project Structure
 
```
Assets/
├── Scripts/
│   ├── BoardManager.cs   # Board state, match detection, gravity, refill, scoring
│   ├── Piece.cs          # Swipe input, movement lerp, piece type definition
│   └── UIManager.cs      # Score and move counter UI updates
├── Prefabs/
│   ├── Blue.prefab       # Blue piece variant
│   ├── Green.prefab      # Green piece variant
│   ├── Red.prefab        # Red piece variant
│   ├── Yellow.prefab     # Yellow piece variant
│   ├── Purple.prefab     # Purple piece variant
│   ├── Pink.prefab       # Pink piece variant
│   └── Box.prefab        # Box piece variant
├── Scenes/
│   └── GameScene.unity   # Main game scene
├── Sprites/              # Piece and background artwork
└── UI/                   # Canvas, TextMeshPro references
```
 
## 🔧 Technical Stack
 
- **Engine:** Unity 2021.3 LTS or later
- **UI:** TextMeshPro for score and move counter rendering
- **Animation:** Code-driven `Vector2.Lerp` for all piece movement (no Animator)
- **Input:** Legacy `OnMouseDown` / `OnMouseUp` with manual swipe angle calculation
- **Board Logic:** 2D array (`Piece[,]`) with coroutine-sequenced processing pipeline
## 📝 Development Notes
 
- **Tuple Deconstruction:** C# value-tuple syntax `(a, b) = (b, a)` is used throughout for clean, allocation-free swaps of positions, board references, and location metadata.
- **No Physics on Pieces:** Pieces are purely transform-based; gravity is simulated logically in the array and visually via Lerp, avoiding Rigidbody2D overhead.
- **Coroutine Pipeline:** `ProcessBoardRoutine` chains `DestroyMatches → ApplyGravity → RefillBoard` with small delays between each phase to give animations time to complete before the next step runs.
- **State Guard:** Every input entry point checks `currentState == GameState.Move` first, making it safe to run async coroutines without additional locks.
- **Prefab-Driven Types:** `PieceType` is read from each prefab's `Piece` component, so adding a new color requires only a new prefab — no code changes needed.
---
 
*This project was developed as a technical study for match-3 puzzle game implementation, board algorithm design, and cascade scoring systems.*
