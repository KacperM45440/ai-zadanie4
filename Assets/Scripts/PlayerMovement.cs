using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroupMovement : MonoBehaviour
{
    [SerializeField] private Camera cameraTransform;
    [SerializeField] private float agentSpacing;
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;

    [SerializeField] private List<NavMeshAgent> groupMembers;

    private void Start()
    {
        RandomizeAgentSpeeds();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MoveGroupToDestination();
        }

        UpdateCameraPosition();
        CheckAgentMovement();
    }

    private void RandomizeAgentSpeeds()
    {
        foreach (var agent in groupMembers)
        {
            agent.speed = Random.Range(minSpeed, maxSpeed);
        }
    }

    private void MoveGroupToDestination()
    {
        Ray ray = cameraTransform.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, agentSpacing, NavMesh.AllAreas))
            {
                SetGroupTarget(navHit.position);
            }
        }
    }

    private void SetGroupTarget(Vector3 targetPosition)
    {
        for (int i = 0; i < groupMembers.Count; i++)
        {
            Vector3 targetWithOffset = targetPosition + Random.insideUnitSphere * agentSpacing / 2;
            Vector3 adjustedTarget = AdjustPositionForSpacing(targetWithOffset, i);

            if (NavMesh.SamplePosition(adjustedTarget, out NavMeshHit navHit, agentSpacing, NavMesh.AllAreas))
            {
                groupMembers[i].SetDestination(navHit.position);
            }
        }
    }

    private Vector3 AdjustPositionForSpacing(Vector3 target, int agentIndex)
    {
        for (int j = 0; j < agentIndex; j++)
        {
            Vector3 directionToAgent = (groupMembers[agentIndex].transform.position - groupMembers[j].transform.position).normalized;
            if (Vector3.Distance(groupMembers[agentIndex].transform.position, groupMembers[j].transform.position) < agentSpacing)
            {
                target += directionToAgent * agentSpacing;
            }
        }

        return target;
    }

    private void CheckAgentMovement()
    {
        foreach (var agent in groupMembers)
        {
            agent.isStopped = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
        }
    }

    private void UpdateCameraPosition()
    {
        if (groupMembers.Count > 0)
        {
            cameraTransform.transform.position = new Vector3(groupMembers[0].transform.position.x, cameraTransform.transform.position.y, groupMembers[0].transform.position.z - 15f);
        }
    }
}
