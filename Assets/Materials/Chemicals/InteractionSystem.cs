using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    public Camera mainCamera;
    public float interactionRange = 2.0f;
    
    private GameObject selectedObject = null;
    private GameObject heldObject = null;
    private Vector3 holdOffset = new Vector3(0, 0.5f, 0.5f);
    
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // Highlight hover object
        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            if (hit.collider.gameObject != selectedObject)
            {
                // Unhighlight previous
                if (selectedObject != null)
                    SetHighlight(selectedObject, false);
                
                // Highlight new
                selectedObject = hit.collider.gameObject;
                SetHighlight(selectedObject, true);
            }
        }
        else if (selectedObject != null)
        {
            SetHighlight(selectedObject, false);
            selectedObject = null;
        }
        
        // Pick up / place objects
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject == null && selectedObject != null)
            {
                // Pick up
                heldObject = selectedObject;
                heldObject.GetComponent<Rigidbody>().isKinematic = true;
            }
            else if (heldObject != null)
            {
                // Place down
                heldObject.GetComponent<Rigidbody>().isKinematic = false;
                heldObject = null;
            }
        }
        
        // Move held object
        if (heldObject != null)
        {
            Vector3 targetPosition = mainCamera.transform.position + 
                                    mainCamera.transform.forward * interactionRange * 0.5f +
                                    mainCamera.transform.right * holdOffset.x +
                                    mainCamera.transform.up * holdOffset.y +
                                    mainCamera.transform.forward * holdOffset.z;
                                    
            heldObject.transform.position = Vector3.Lerp(
                heldObject.transform.position, 
                targetPosition, 
                Time.deltaTime * 10.0f);
        }
        
        // Pour or combine (right mouse)
        if (Input.GetMouseButtonDown(1) && heldObject != null && selectedObject != null)
        {
            Container sourceContainer = heldObject.GetComponent<Container>();
            Container targetContainer = selectedObject.GetComponent<Container>();
            
            if (sourceContainer != null && targetContainer != null)
            {
                // Pour 10mL by default
                float amountToPour = 10.0f;
                
                if (sourceContainer.currentVolume < amountToPour)
                    amountToPour = sourceContainer.currentVolume;
                    
                if (targetContainer.currentVolume + amountToPour > targetContainer.maxVolume)
                    amountToPour = targetContainer.maxVolume - targetContainer.currentVolume;
                    
                // Transfer each chemical
                foreach (var mixture in sourceContainer.contents)
                {
                    float ratio = mixture.volume / sourceContainer.currentVolume;
                    float transferAmount = amountToPour * ratio;
                    
                    targetContainer.AddChemical(mixture.chemical, transferAmount);
                    mixture.volume -= transferAmount;
                }
                
                // Update source container
                sourceContainer.currentVolume -= amountToPour;
                sourceContainer.UpdateLiquidVisualization();
                
                // Check for reactions
                FindObjectOfType<ReactionManager>().CheckForReaction(
                    targetContainer, 
                    IsContainerHeated(targetContainer), 
                    null);
            }
        }
    }
    
    private void SetHighlight(GameObject obj, bool highlight)
    {
        // Change material or add outline
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (highlight)
                renderer.material.SetFloat("_Outline", 1.0f);
            else
                renderer.material.SetFloat("_Outline", 0.0f);
        }
    }
    
    private bool IsContainerHeated(Container container)
    {
        // Check if container is on a hotplate or over a bunsen burner
        // Simple implementation: check if it's close to a heat source
        Collider[] colliders = Physics.OverlapSphere(container.transform.position, 0.5f);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("HeatSource"))
                return true;
        }
        return false;
    }
}