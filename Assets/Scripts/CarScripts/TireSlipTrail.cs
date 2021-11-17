using UnityEngine;

namespace MiniRacing.CarScripts
{
    /// <summary>
    /// Tire slip trace immitating object. Works based on state machine to prevent artifacts 
    /// caused by instant processing of emitting and changing position of trail-renderer's gameobject.
    /// </summary>
    public class TireSlipTrail : MonoBehaviour
    {
        private enum State
        {
            BEFORE_EMITTING,
            EMITTING,
            BEFORE_NOT_EMITTING,
            NOT_EMITTING,
        }

        private enum InputSignal
        {
            START_EMITTING,
            STOP_EMITTING,
            NEXT_FRAME
        }

        State state = State.NOT_EMITTING;
        InputSignal signal = InputSignal.NEXT_FRAME;

        private Material material;
        private float trailWidth = 0.2f;
        private float traceDisappearTime = 30f;
        private float distanceBetweenTracePoints = 0.2f;
        private float unitsBeyondSurface = 0.01f;

        private TrailRenderer trailRenderer;
        private Transform trailRendererCarrier;

        public TrailRenderer TrailRenderer { get => trailRenderer; }
        public Transform TrailRendererCarrier { get => trailRendererCarrier; }

        public Material Material
        {
            get { return material; }
            set
            {
                material = value;
                if (trailRenderer != null)
                {
                    trailRenderer.materials = new Material[] { Material };
                }
            }
        }
        internal float TrailWidth
        {
            get { return trailWidth; }
            set {
                this.trailWidth = value;
                if (trailRenderer != null)
                {
                    trailRenderer.widthCurve = AnimationCurve.Constant(0.0f, 0.0f, value);
                }
            }
        }
        internal float TraceDisappearTime
        {
            get { return traceDisappearTime; }
            set
            {
                this.traceDisappearTime = value;
                if (trailRenderer != null)
                {
                    trailRenderer.time = value;
                }
            }
        }
        internal float DistanceBetweenTracePoints
        {
            get
            {
                return distanceBetweenTracePoints;
            }
            set
            {
                distanceBetweenTracePoints = value;
                if (trailRenderer != null)
                {
                    trailRenderer.minVertexDistance = value;
                }
            }
        }
        internal float UnitsBeyondSurface { get => unitsBeyondSurface; set => unitsBeyondSurface = value; }

        void Awake()
        {
            trailRendererCarrier = new GameObject("trailRendererCarrier").transform;
            trailRendererCarrier.parent = null;
            trailRendererCarrier.transform.position = this.transform.position;
            trailRendererCarrier.rotation = Quaternion.FromToRotation(Vector3.back, Vector3.up); // For rotation trailRender's Z-alignment to Y-alignment
            trailRenderer = trailRendererCarrier.gameObject.AddComponent<TrailRenderer>();
            trailRenderer.materials = new Material[] { Material };
            trailRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            trailRenderer.receiveShadows = true;
            trailRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            trailRenderer.time = traceDisappearTime;
            trailRenderer.widthCurve = AnimationCurve.Constant(0.0f, 0.0f, trailWidth);
            trailRenderer.startWidth = trailWidth;
            trailRenderer.endWidth = trailWidth;
            trailRenderer.minVertexDistance = distanceBetweenTracePoints;
            trailRenderer.autodestruct = false;
            trailRenderer.emitting = false;
            trailRenderer.alignment = LineAlignment.TransformZ;
            trailRenderer.textureMode = LineTextureMode.Tile;
            trailRenderer.allowOcclusionWhenDynamic = false;
            trailRenderer.numCapVertices = 10;
            trailRenderer.numCornerVertices = 10;

        }

        public bool isEmitting()
        {
            return trailRenderer.emitting;
        }

        public void SetPosition(Vector3 pos)
        {
            trailRendererCarrier.position = pos + unitsBeyondSurface * Vector3.up;
        }

        public void StartEmitting()
        {
            trailRenderer.emitting = true;
            //trailRendererCarrier.position = startPosition + unitsBeyondSurface * Vector3.up;
            //signal = InputSignal.START_EMITTING;
        }

        public void StopEmitting()
        {
            trailRenderer.emitting = false;
            //signal = InputSignal.STOP_EMITTING;
        }

        void Start()
        {
            //StartCoroutine(trailLoop());
        }

        /*IEnumerator trailLoop()
        {
            while(true)
            {
                switch (signal)
                {
                    case InputSignal.START_EMITTING:
                        if (state == State.NOT_EMITTING || state == State.BEFORE_NOT_EMITTING)
                            state = State.BEFORE_EMITTING;
                        break;

                    case InputSignal.STOP_EMITTING:
                        if (state == State.BEFORE_EMITTING || state == State.EMITTING)
                            state = State.BEFORE_NOT_EMITTING;
                        break;

                    default:
                        Debug.LogError("Got unknown signal: " + signal.ToString());
                        break;
                }

                switch (state)
                {
                    case State.EMITTING:
                        //trailRenderer.emitting = false;
                        trailRenderer.emitting = true;
                        trailRendererCarrier.position = this.startPos + unitsBeyondSurface * Vector3.up;
                        break;
                    case State.BEFORE_EMITTING:
                        trailRenderer.emitting = false;
                        yield return new WaitForEndOfFrame();
                        yield return new WaitForFixedUpdate();
                        trailRendererCarrier.position = this.startPos + unitsBeyondSurface * Vector3.up;

                        yield return new WaitForEndOfFrame();
                        yield return new WaitForFixedUpdate();
                        state = State.EMITTING;
                        break;
                    case State.NOT_EMITTING:
                        trailRenderer.emitting = false;
                        break;
                    case State.BEFORE_NOT_EMITTING:
                        trailRendererCarrier.position = this.startPos + unitsBeyondSurface * Vector3.up;
                        trailRenderer.emitting = false;
                        state = State.NOT_EMITTING;
                        break;
                    default:
                        Debug.LogError("Got unknown state: " + state.ToString());
                        break;
                }

                yield return new WaitForEndOfFrame();
            }
        }*/

        // Update is called once per frame
        void FixedUpdate()
        {
            /*switch (state)
            {
                case State.EMITTING:
                    //trailRenderer.emitting = false;
                    trailRenderer.emitting = true;
                    trailRendererCarrier.position = this.startPos + unitsBeyondSurface * Vector3.up;
                    Debug.Log("State = " + state + "; Input = " + signal + "; Emitting = " + trailRenderer.emitting);
                    Debug.Log(trailRendererCarrier.position);
                    Debug.DrawRay(trailRendererCarrier.position, Vector3.up, Color.white, 60);
                    break;
                case State.BEFORE_EMITTING:
                    trailRenderer.emitting = false;
                    trailRendererCarrier.position = this.startPos + unitsBeyondSurface * Vector3.up;
                    //trailRenderer.emitting = false;
                    Debug.Log("State = " + state + "; Input = " + signal + "; Emitting = " + trailRenderer.emitting);
                    Debug.Log(trailRendererCarrier.position);
                    Debug.DrawRay(trailRendererCarrier.position, Vector3.up, Color.blue, 60);
                    state = State.EMITTING;
                    break;
                case State.NOT_EMITTING:
                    trailRenderer.emitting = false;
                    break;
                case State.BEFORE_NOT_EMITTING:
                    trailRendererCarrier.position = this.startPos + unitsBeyondSurface * Vector3.up;
                    trailRenderer.emitting = false;
                    Debug.Log("State = " + state + "; Input = " + signal + "; Emitting = " + trailRenderer.emitting);
                    Debug.Log(trailRendererCarrier.position);
                    Debug.DrawRay(trailRendererCarrier.position, Vector3.up, Color.red, 60);
                    state = State.NOT_EMITTING;
                    break;
                default:
                    Debug.LogError("Got unknown state: " + state.ToString());
                    break;
            }

            switch (signal)
            {
                case InputSignal.START_EMITTING:
                    if (state == State.NOT_EMITTING || state == State.BEFORE_NOT_EMITTING)
                        state = State.BEFORE_EMITTING;
                    break;

                case InputSignal.STOP_EMITTING:
                    if (state == State.BEFORE_EMITTING || state == State.EMITTING)
                        state = State.BEFORE_NOT_EMITTING;
                    //else if (state == State.BEFORE_NOT_EMITTING || state == State.NOT_EMITTING)
                    //    state = State.NOT_EMITTING;
                    break;

                //case InputSignal.NEXT_FRAME:
                //    if (state == State.BEFORE_NOT_EMITTING)
                //        state = State.NOT_EMITTING;
                //    else if (state == State.BEFORE_EMITTING)
                //        state = State.EMITTING;
                //    break;

                default:
                    Debug.LogError("Got unknown signal: " + signal.ToString());
                    break;
            }*/

        }
    }
}