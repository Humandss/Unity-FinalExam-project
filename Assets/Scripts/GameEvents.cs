using System;

// Observer pattern (lightweight static event bus).
//
// Decouples "an enemy died, worth N points" from "the score UI updates".
// Before, an enemy called Score_Manager.instance.IncreaseScore(), and that
// method read EnemyFSM.instance.enemyScore - a global that every enemy
// overwrote in Awake, so the score always used the LAST spawned enemy's value.
// Now the dying enemy publishes its OWN score and any number of listeners
// (score UI, achievements, sound, ...) can react without the enemy knowing
// they exist.
//
// Pitfall this introduces: a static event lives across scene loads, so every
// subscriber MUST unsubscribe (see Score_Manager.OnDisable) or it will fire
// multiple times after a restart. This trade-off is discussed in the report.
public static class GameEvents
{
    // Raised when an enemy dies. Argument = score awarded for that enemy.
    public static event Action<int> EnemyKilled;

    public static void RaiseEnemyKilled(int score)
    {
        EnemyKilled?.Invoke(score);
    }
}
