using UnityEngine;

public class PlinkoInitializer : MonoBehaviour
{
    [SerializeField] private GameObject plinkoColliderPrefab;
    [SerializeField] private int lineCount = 8;
    [SerializeField] private float horizontalSpacing = 1f;
    [SerializeField] private float verticalSpacing = 1f;
    [SerializeField] private Vector2 startPosition = new Vector2(0f, 0f);

    private void Start()
    {
        InitializePlinkoBoard();
    }

    private void InitializePlinkoBoard()
    {
        for (int line = 0; line < lineCount; line++)
        {
            // Start with 3 pegs and increase by 1 each line
            int pegsInCurrentLine = 3 + line;
            float lineWidth = (pegsInCurrentLine - 1) * horizontalSpacing;
            float startX = startPosition.x - (lineWidth / 2f);
            float currentY = startPosition.y - (line * verticalSpacing);

            for (int peg = 0; peg < pegsInCurrentLine; peg++)
            {
                Vector2 pegPosition = new Vector2(
                    startX + (peg * horizontalSpacing),
                    currentY
                );

                GameObject newPeg = Instantiate(
                    plinkoColliderPrefab,
                    pegPosition,
                    Quaternion.identity,
                    transform
                );

                newPeg.name = $"Plinko_Line{line}_Peg{peg}";
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        
        for (int line = 0; line < lineCount; line++)
        {
            int pegsInCurrentLine = 3 + line;
            float lineWidth = (pegsInCurrentLine - 1) * horizontalSpacing;
            float startX = startPosition.x - (lineWidth / 2f);
            float currentY = startPosition.y - (line * verticalSpacing);

            for (int peg = 0; peg < pegsInCurrentLine; peg++)
            {
                Vector2 pegPosition = new Vector2(
                    startX + (peg * horizontalSpacing),
                    currentY
                );
                
                Gizmos.DrawWireSphere(pegPosition, 0.1f);
            }
        }
    }
}