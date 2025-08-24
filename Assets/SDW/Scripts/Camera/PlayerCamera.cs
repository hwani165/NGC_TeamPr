using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float _minCameraSize = 6.5f;
    [SerializeField] private float _maxCameraSize = 8f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _basu = 0.5f;

    [SerializeField] private Transform _player1;
    [SerializeField] private Transform _player2;

    [SerializeField] private Vector2 _borderSize = new Vector2(4f, 2f); // ī�޶� ���� ����

    private Camera _camera;
    private Vector3 _targetPos;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _targetPos = transform.position; // ������ �� ���� ��ġ�� ��������
    }

    private void FixedUpdate()
    {
        // �÷��̾� �߰���
        Vector3 midPos = new Vector3(
            (_player1.position.x + _player2.position.x) / 2,
            (_player1.position.y + _player2.position.y) / 2,
            -10f);

        // ���� ī�޶� �߽ɰ� �÷��̾� �߰��� ����
        Vector3 offset = midPos - _targetPos;

        // X�� üũ
        if (Mathf.Abs(offset.x) > _borderSize.x)
        {
            _targetPos.x = midPos.x - Mathf.Sign(offset.x) * _borderSize.x;
        }

        // Y�� üũ
        if (Mathf.Abs(offset.y) > _borderSize.y)
        {
            _targetPos.y = midPos.y - Mathf.Sign(offset.y) * _borderSize.y;
        }

        transform.position = Vector3.Lerp(transform.position, _targetPos, _speed * Time.fixedDeltaTime);

        // �� (�� �÷��̾� ���� �Ÿ� ���)
        float targetSize = Vector2.Distance(_player1.position, _player2.position) * _basu;
        _camera.orthographicSize = Mathf.Clamp(
            Mathf.Lerp(_camera.orthographicSize, targetSize, _speed * Time.fixedDeltaTime),
            _minCameraSize, _maxCameraSize);
    }
}
