using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollGestureZone : MonoBehaviour
{
    public event System.Action OnUserInput;

    [SerializeField] private Renderer visualRenderer;
    [SerializeField] private Material activeMaterial;
    [SerializeField] private Vector3 direction = Vector3.right;
    [SerializeField] private float SmoothingRate = 0.1f;
    [SerializeField] private float DecayRate = 0.05f;
    [SerializeField] private float Speed = 1;

    public float DebugRayScalar = 100;

    private Material inactiveMaterial;
    private Collider activeCollider;
    private Hand activeHand;
    private Vector3 lastHandPosition;
    private Collider zoneCollider;
    private Vector3 localHandContactPoint; //Local to the hand

    public MomentumFloat DirectionDeltaMomentum { get; private set; }
    public Vector3 TrackPosition { get; private set; }
    public Vector3 TrackDelta { get; private set; }
    public float ScrollValue { get; private set; }
    public bool IsTrackActive => activeHand != null;

    private void Awake()
    {
        zoneCollider = GetComponent<Collider>();
        inactiveMaterial = visualRenderer.sharedMaterial;
        DirectionDeltaMomentum = new MomentumFloat(DecayRate, SmoothingRate);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsTrackActive) return;

        

        var hand = other.gameObject.GetComponentInParent<Hand>();
        if(hand != null)
        {
            var contactPoint = zoneCollider.ClosestPoint(other.transform.position);
            
            visualRenderer.sharedMaterial = activeMaterial;
            activeCollider = other;
            activeHand = hand;
            //TrackPosition = activeHand.transform.position;
            TrackPosition = contactPoint;
            localHandContactPoint = hand.transform.InverseTransformPoint(contactPoint);
            TrackDelta = Vector3.zero;
            DirectionDeltaMomentum.Set(0);
            lastHandPosition = TrackPosition;

            OnUserInput?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other == activeCollider)
        {
            visualRenderer.sharedMaterial = inactiveMaterial;
            activeCollider = null;
            activeHand = null;
        }
    }

    private void FixedUpdate()
    {
        DirectionDeltaMomentum.SmoothingRate = SmoothingRate;
        DirectionDeltaMomentum.DecayRate = DecayRate;

        DirectionDeltaMomentum.Update();
        if (activeHand != null)
        {
            //TrackPosition = activeHand.transform.position;
            TrackPosition = activeHand.transform.TransformPoint(localHandContactPoint);
            TrackDelta = TrackPosition - lastHandPosition;

            DirectionDeltaMomentum.Set(Vector3.Dot(transform.TransformDirection(direction), TrackDelta) * Speed);

            lastHandPosition = TrackPosition;
        }

        ScrollValue = DirectionDeltaMomentum.Value;
    }
}
