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

    public AudioSource sfxSource;      // 효과음을 재생할 AudioSource
    public AudioClip sfx_Maga;         // "마가"
    public AudioClip sfx_Hongcha;      // "홍차 건네기"
    public AudioClip sfx_GwansaeUp;    // "관세up"
    public AudioClip sfx_BearRip;      // "곰은 사람을 찢어"

    void Start()
    {
    }

    void Update()
    {
    }

    void OnGUI() // 거의 매 프레임마다 호출됨
    {
        // 두 카드 다 인식 + 준비상태일 때
        if (obj_Trump.is_detected == true && obj_Putin.is_detected == true && game_state_ == eGameState.Ready)
        {
            if (BatteleLogText)
            {
                BatteleLogText.text =
                    "두 거물 카드 인식 완료!\n" +
                    "트럼푸와 푸튄, VR 세계에 공식 접속되었습니다.\n" +
                    "이제 지구 1짱을 가리는 사상 최초의 VR 결승전을 시작합니다.\n" +
                    "최종결정자 AI '재맹': \"승자는 현실로,\n 패자는 평생 접속 유지입니다.\"";
                game_state_ = eGameState.Battle;
                StartCoroutine(RollDice());
            }
        }

        // 아직 준비 상태일 때
        if (game_state_ == eGameState.Ready)
        {
            if (BatteleLogText)
            {
                BatteleLogText.text =
                    "[대기 중] VR '지구 1짱 결승전' 접속 대기...\n" +
                    "트럼푸와 푸튄 카드를 동시에 카메라에 인식시켜 주세요.";
            }
        }
    }

    // 공용 효과음 재생 함수
    void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }

    IEnumerator RollDice()
    {
        BatteleLogText.text =
            "선공을 정하기 위해 운명 주사위를 굴립니다...\n" +
            "AI 재맹: \"룰은 공정하게, 결과는 냉정하게.\n참가자의 멘탈은… 알아서 챙기세요.\"";
        yield return new WaitForSeconds(1.5f);

        int last_trump_dice = 0;
        int last_putin_dice = 0;
        
        for (int i = 0; i < 20; i++)
        {
            last_trump_dice = Random.Range(1, 7);
            last_putin_dice = Random.Range(1, 7);

            if (BatteleLogText)
            {
                BatteleLogText.text =
                    "트럼푸 주사위: " + last_trump_dice + "  푸튄 주사위: " + last_putin_dice +
                    "\n관전자 채팅: \"와 이거 진짜 실시간 맞냐 ㅋㅋ\"";
            }

            yield return new WaitForSeconds(0.1f);
        }
        
        if (last_trump_dice > last_putin_dice)
        {
            // 트럼푸 선공
            if (BatteleLogText)
            {
                BatteleLogText.text =
                    "트럼푸 선공!\n" +
                    "AI 재맹: \"마이크를 먼저 쥔 자, 지구 1짱에 더 가깝습니다.\n" +
                    "다만 너무 떠들다 보면 패배 인터뷰도 같이 예약될 수 있습니다.\"";
                currentPlayerTurn = ePlayerTurn.Trump;
            }
        }
        else if (last_trump_dice < last_putin_dice)
        {
            // 푸튄 선공
            if (BatteleLogText)
            {
                BatteleLogText.text =
                    "푸튄 선공!\n" +
                    "AI 재맹: \"조용히 시작하지만, 마지막에 웃는 건 누구일까요?\"\n" +
                    "관전자 채팅: \"재맹이햄 폼 미쳤다 ㄷㄷ\"";
                currentPlayerTurn = ePlayerTurn.Putin;
            }
        }
        else
        {
            // 동점
            if (BatteleLogText)
            {
                BatteleLogText.text =
                    "동점입니다.\n" +
                    "AI 재맹: \"민주주의를 존중해서 한 번 더 굴리겠습니다.\"\n" +
                    "시청자들: \"이 정도면 주사위도 정치적 중립이네 ㅋㅋ\"";
                yield return new WaitForSeconds(1.0f);
                StartCoroutine(RollDice());
            }
        }
    }
    
    // 트럼프(트럼푸) 공격 버튼에 연결할 함수
    public void OnTrumpAttack(int Tpye)
    {
        if (game_state_ == eGameState.Attacking) return;

        if (game_state_ == eGameState.Battle && currentPlayerTurn == ePlayerTurn.Trump)
        {
            game_state_ = eGameState.Attacking;

            int damage = 0;

            switch (Tpye)
            {
                // 1번 스킬: 러시아 관세 올리기
                case 1:
                    damage = 50;
                    BatteleLogText.text =
                        "트럼푸가 스킬 [러시아 관세 올리기]를 사용했습니다!\n" +
                        "VR 세계에서 러시아산 수입품 관세를 급격히 인상합니다.\n" +
                        "푸튄의 경제 멘탈이 흔들리며 VR HP에 50의 데미지가 들어갑니다.";
                    SetActiveModel(obj_Trump, 1);
                    PlaySFX(sfx_GwansaeUp);
                    break;

                // 2번 스킬: 비트코인 올리기
                case 2:
                    damage = 80;
                    BatteleLogText.text =
                        "트럼푸가 궁극기 [비트코인 올리기]를 발동했습니다!\n" +
                        "의문의 한 마디로 전 세계 코인 시장이 요동칩니다.\n" +
                        "푸튄의 시스템 안정성이 80만큼 붕괴되고,\n" +
                        "관전자 채팅: \"내 코인까지 같이 맞았는데요? ㅋㅋ\"";
                    SetActiveModel(obj_Trump, 2);
                    PlaySFX(sfx_Maga);
                    break;
            }
            
            StartCoroutine(ProcessAttack(ePlayerTurn.Trump, damage));
        }
    }

    // 푸틴(푸튄) 공격 버튼에 연결할 함수
    public void OnPutinAttack(int Tpye)
    {
        if (game_state_ == eGameState.Attacking) return;

        if (game_state_ == eGameState.Battle && currentPlayerTurn == ePlayerTurn.Putin)
        {
            game_state_ = eGameState.Attacking;
            
            int damage = 0;

            switch (Tpye)
            {
                // 1번 스킬: 홍차 건네기
                case 1:
                    damage = 45;
                    BatteleLogText.text =
                        "푸튄이 스킬 [홍차 건네기]를 사용했습니다!\n" +
                        "트럼푸에게 따뜻한 홍차를 건네며 의미심장한 미소를 짓습니다.\n" +
                        "트럼푸의 경계심이 풀리며 자존심에 45의 데미지가 들어갑니다.";
                    SetActiveModel(obj_Putin, 1);
                    PlaySFX(sfx_Hongcha);
                    break;

                // 2번 스킬: 곰은 사람을 찢어
                case 2:
                    damage = 70;
                    BatteleLogText.text =
                        "푸튄이 궁극기 [곰은 사람을 찢어]를 발동했습니다!\n" +
                        "거대한 VR 곰이 소환되어 트럼푸의 아바타를 마구 끌어안고 휘두릅니다.\n" +
                        "트럼푸의 VR HP에 70의 데미지가 꽂히고, 관전자 채팅창이 폭발합니다.";
                    SetActiveModel(obj_Putin, 2);
                    PlaySFX(sfx_BearRip);
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
            if (putinHP < 0) putinHP = 0;
            putinHPText.text = "푸튄\nVR HP: " + putinHP;
        }
        else
        {
            trumpHP -= damage;
            if (trumpHP < 0) trumpHP = 0;
            trumpHPText.text = "트럼푸\nVR HP: " + trumpHP;
        }
        
        yield return new WaitForSeconds(2f);

        // 게임 종료 확인
        if (putinHP <= 0)
        {
            putinHP = 0;
            game_state_ = eGameState.End;

            if (BatteleLogText)
            {
                BatteleLogText.text =
                    "트럼푸 최종 승리!\n" +
                    "AI 재맹: \"지구 1짱은 트럼푸로 확정되었습니다.\"\n" +
                    "트럼푸는 VR 세계에서 로그아웃하고 현실로 복귀합니다.";
                SetActiveModel(obj_Trump, 3);
                SetActiveModel(obj_Putin, 4);
            }
        }
        else if (trumpHP <= 0)
        {
            trumpHP = 0;
            game_state_ = eGameState.End;

            if (BatteleLogText)
            {
                BatteleLogText.text =
                    "푸튄 최종 승리!\n" +
                    "AI 재맹: \"지구 1짱은 푸튄으로 확정되었습니다.\"\n" +
                    "트럼푸의 아바타는 VR 세계에 영구 접속 처리됩니다...";
                SetActiveModel(obj_Trump, 4);
                SetActiveModel(obj_Putin, 3);
            }
        }
        else
        {
            // 둘 다 안 죽었으면 턴 교체
            SwitchTurn();
        }
    }

    void SwitchTurn()
    {
        if (currentPlayerTurn == ePlayerTurn.Trump)
        {
            currentPlayerTurn = ePlayerTurn.Putin;
            BatteleLogText.text =
                "푸튄의 턴입니다.\n" +
                "AI 재맹: \"조용히 계산을 끝낸 자가 마지막에 웃을 수도 있습니다.\"";
        }
        else
        {
            currentPlayerTurn = ePlayerTurn.Trump;
            BatteleLogText.text =
                "트럼푸의 턴입니다.\n" +
                "AI 재맹: \"발언권을 쥔 자, 흐름을 뒤집을 시간입니다.\"";
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
