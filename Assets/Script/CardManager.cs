using UnityEngine;
using UnityEngine.UI; // UI 네임스페이스 추가
using System.Collections; // 코루틴을 사용하려면 필요합니다.

public enum eGameState
{
    Ready = 0,
    Battle,
    Attacking,
    End
}

public enum ePlayerTurn
{
    Trump,
    Putin
}

public class CardManager : MonoBehaviour
{
    public TrackingObject obj_Trump;
    public TrackingObject obj_Putin;
    public eGameState game_state_ = eGameState.Ready;
    
    public Text BatteleLogText; // 전투 결과를 표시할 UI 텍스트
    
    public TextMesh trumpHPText;
    public TextMesh putinHPText;
    public int trumpHP = 300;
    public int putinHP = 300;
    public ePlayerTurn currentPlayerTurn;
    
    public int winner = -1; // 0: 트럼프 승리, 1: 푸틴 승리, -1: 진행 중
    
    void Start()
    {
    }
    void Update()
    {
    }

    void OnGUI() // 거의 매 프레임마다 호출됨
    {
        if( obj_Trump.is_detected == true && obj_Putin.is_detected == true && game_state_ == eGameState.Ready )
        {
            if (BatteleLogText)
            {
                BatteleLogText.text = "두 카드가 모두 인식되었습니다.";
                game_state_ = eGameState.Battle;
                StartCoroutine(RollDice());
            }
        }

        if (game_state_ == eGameState.Ready)
        {
            if (BatteleLogText)
            {
                BatteleLogText.text = "[준비 상태] 두 카드를 인식시켜 주세요.";
            }
        }
        else if (game_state_ == eGameState.End)
        {
            if (BatteleLogText)
            {
                if (winner == 0) // 트럼프 승리
                {   
                    BatteleLogText.text = "트럼프 최종 승리!";
                }
                else if (winner == 1) // 푸틴 승리
                {
                    BatteleLogText.text = "푸틴 최종 승리!";
                }
            }
        }
    }

    IEnumerator RollDice()
    {
        BatteleLogText.text = "선공을 정하기 위해 주사위를 굴립니다...";
        yield return new WaitForSeconds(1.5f);

        int last_trump_dice = 0;
        int last_putin_dice = 0;
        
        for (int i = 0; i < 20; i++)
        {
            last_trump_dice = Random.Range(1, 7);
            last_putin_dice = Random.Range(1, 7);
            if (BatteleLogText)
            {
                BatteleLogText.text = "트럼프 주사위: " + last_trump_dice + "  푸틴 주사위: " + last_putin_dice;
            }
            yield return new WaitForSeconds(0.1f);
        }
        
        if (last_trump_dice > last_putin_dice)
        {
            if (BatteleLogText)
            {
                BatteleLogText.text = "트럼프 선공! 트럼프의 턴입니다.";
            }
        }
        else if (last_trump_dice < last_putin_dice)
        {
            if (BatteleLogText)
            {
                BatteleLogText.text = "푸틴 선공! 푸틴의 턴입니다.";
            }
        }
        else
        {
            if (BatteleLogText)
            {
                BatteleLogText.text = "무승부! 다시 주사위를 굴립니다.";
                yield return new WaitForSeconds(1.0f);
                StartCoroutine(RollDice());
            }
        }
    }
    
    // 트럼프 공격 버튼에 연결할 함수
    public void OnTrumpAttack( int Tpye )
    {
        if(game_state_ == eGameState.Attacking) return;
        if (game_state_ == eGameState.Battle && currentPlayerTurn == ePlayerTurn.Trump)
        {
            game_state_ = eGameState.Attacking;

            int damage = 0;
            switch (Tpye)
            {  
                case 1:
                    damage = 50;
                    BatteleLogText.text = "트럼프가 푸틴에게 50의 피해를 입혔습니다!";
                    SetActiveModel(obj_Trump, 1);
                    break;
                case 2:
                    damage = 80;
                    BatteleLogText.text = "트럼프가 푸틴에게 80의 피해를 입혔습니다!";
                    SetActiveModel(obj_Trump, 2);
                    break;
            }
            
            StartCoroutine(ProcessAttack(ePlayerTurn.Trump, damage));
        }
    }

    // 푸틴 공격 버튼에 연결할 함수
    public void OnPutinAttack( int Tpye )
    {
        if(game_state_ == eGameState.Attacking) return;
        if (game_state_ == eGameState.Battle && currentPlayerTurn == ePlayerTurn.Putin)
        {
            game_state_ = eGameState.Attacking;
            
            int damage = 0;
            switch (Tpye)
            {  
                case 1:
                    damage = 45;
                    BatteleLogText.text = "푸틴이 트럼프에게 45의 피해를 입혔습니다!";
                    SetActiveModel(obj_Putin, 1);
                    break;
                case 2:
                    damage = 70;
                    BatteleLogText.text = "푸틴이 트럼프에게 70의 피해를 입혔습니다!";
                    SetActiveModel(obj_Putin, 2);
                    break;
            }
            
            StartCoroutine(ProcessAttack(ePlayerTurn.Putin, damage));
        }
    }

    // 공격 처리 코루틴
    IEnumerator ProcessAttack(ePlayerTurn attacker, int damage)
    {
 
        // 피해 적용
        if (attacker == ePlayerTurn.Trump)
        {
            putinHP -= damage;
            putinHPText.text = "푸틴\nHP: " + putinHP;
        }
        else
        {
            trumpHP -= damage;
            trumpHPText.text = "트럼프\nHP: " + trumpHP;
        }
        
        yield return new WaitForSeconds(2f);

        // 게임 종료 확인
        if (putinHP <= 0)
        {
            putinHP = 0;
            BatteleLogText.text = "트럼프 최종 승리!";
            game_state_ = eGameState.End;
        }
        else if (trumpHP <= 0)
        {
            trumpHP = 0;
            BatteleLogText.text = "푸틴 최종 승리!";
            game_state_ = eGameState.End;
        }
        else
        {
            SwitchTurn();
        }
    }

    void SwitchTurn()
    {
        if (currentPlayerTurn == ePlayerTurn.Trump)
        {
            currentPlayerTurn = ePlayerTurn.Putin;
            BatteleLogText.text = "푸틴의 턴입니다.";
        }
        else
        {
            currentPlayerTurn = ePlayerTurn.Trump;
            BatteleLogText.text = "트럼프의 턴입니다.";
        }
        
        game_state_ = eGameState.Battle;

        // 각 캐릭터의 0번(기본) 모델을 활성화합니다.
        SetActiveModel(obj_Trump, 0);
        SetActiveModel(obj_Putin, 0);
    }
    
    void SetActiveModel(TrackingObject characterObject, int modelIndex)
    {
        if (characterObject.models == null || characterObject.models.Length <= modelIndex)
        {
            Debug.LogError(characterObject.name + "에 모델이 설정되지 않았거나 인덱스가 잘못되었습니다.");
            return;
        }

        for (int i = 0; i < characterObject.models.Length; i++)
        {
            characterObject.models[i].SetActive(i == modelIndex);
        }
    }
    
}
