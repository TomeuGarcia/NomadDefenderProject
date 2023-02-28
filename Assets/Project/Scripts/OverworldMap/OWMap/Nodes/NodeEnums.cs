using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEnums
{
    public enum NodeType { NONE, BATTLE, UPGRADE }
    public enum ProgressionState { EARLY, MID, LATE }
    public enum HealthState { UNDAMAGED, SLIGHTLY_DAMAGED, GREATLY_DAMAGED, DESTROYED }
    public enum BattleType { EARLY, MID, LATE, BOSS }
    public enum UpgradeType { NEW_TURRET_CARD, REPLACE_ATTACK_PART, REPLACE_BODY_PART, REPLACE_BASE_PART, COUNT } // TODO add more types
    public enum EmptyType { FIRST_LEVEL, LAST_LEVEL, COUNT }
}
