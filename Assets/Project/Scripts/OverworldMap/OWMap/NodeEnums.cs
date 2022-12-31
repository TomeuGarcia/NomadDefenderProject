using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEnums
{
    public enum NodeType { NONE, BATTLE, UPGRADE }
    public enum HealthState { UNDAMAGED, SLIGHTLY_DAMAGED, GREATLY_DAMAGED, DESTROYED }
    public enum BattleType { EARLY, LATE, BOSS }
    public enum UpgradeType { DRAW_A_CARD, REPLACE_CARD_PART } // TODO add more types
}
