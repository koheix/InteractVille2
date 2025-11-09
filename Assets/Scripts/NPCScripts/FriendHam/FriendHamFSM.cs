/* ともハムのFSMを管理するスクリプト
 *
 */

using UnityEngine;

// ともハム会話の状態
public enum FriendHamState
{
    // アイデアが固まるまで保留
    Idle,       // 待機中
    Greeting,   // 挨拶
    AskHelp,    // 手伝いを頼む
    ThankYou,   // お礼
    Goodbye,    // さようなら
    End         // 終了
}

public class FriendHamFSM
{

}
