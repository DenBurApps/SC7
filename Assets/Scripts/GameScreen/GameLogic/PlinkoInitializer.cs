using GameScreen.GameLogic;
using UnityEngine;

public class PlinkoInitializer : MonoBehaviour
{
    [SerializeField] private PlinkoCollider _plinkoColliderPrefab;
    [SerializeField] private int _lineCount = 8;
    [SerializeField] private float _horizontalSpacing = 1f;
    [SerializeField] private float _verticalSpacing = 1f;
    [SerializeField] private Vector2 _startPosition = new Vector2(0f, 0f);
    [SerializeField] private AudioSource _hitSound;

    private void Start()
    {
        InitializePlinkoBoard();
    }

    private void InitializePlinkoBoard()
    {
        for (int line = 0; line < _lineCount; line++)
        {
            int pegsInCurrentLine = 3 + line;
            float lineWidth = (pegsInCurrentLine - 1) * _horizontalSpacing;
            float startX = _startPosition.x - (lineWidth / 2f);
            float currentY = _startPosition.y - (line * _verticalSpacing);

            for (int peg = 0; peg < pegsInCurrentLine; peg++)
            {
                Vector2 pegPosition = new Vector2(
                    startX + (peg * _horizontalSpacing),
                    currentY
                );

                PlinkoCollider newPeg = Instantiate(
                    _plinkoColliderPrefab,
                    pegPosition,
                    Quaternion.identity,
                    transform
                );

                newPeg.AssignHitSound(_hitSound);
                newPeg.name = $"Plinko_Line{line}_Peg{peg}";
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        
        for (int line = 0; line < _lineCount; line++)
        {
            int pegsInCurrentLine = 3 + line;
            float lineWidth = (pegsInCurrentLine - 1) * _horizontalSpacing;
            float startX = _startPosition.x - (lineWidth / 2f);
            float currentY = _startPosition.y - (line * _verticalSpacing);

            for (int peg = 0; peg < pegsInCurrentLine; peg++)
            {
                Vector2 pegPosition = new Vector2(
                    startX + (peg * _horizontalSpacing),
                    currentY
                );
                
                Gizmos.DrawWireSphere(pegPosition, 0.07f);
            }
        }
    }
}