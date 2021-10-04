using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpBetweenTwoPoints : MonoBehaviour
{
    private float _timePerSegments = 10f;
    private float _timeIntoSegments = 5f;

    public List<Transform> Transforms = new List<Transform>();
    private List<Vector3> _positionList;

    private int _LastPos;
    private int _moveTowardsNextPos;

    private void Start()
    {
        MakeVector3List();
    }

    private void MakeVector3List()
    {
        _positionList = new List<Vector3>();
        for (int i = 0; i < Transforms.Count; i++)
        {
            _positionList.Add(Transforms[i].position);
        }
        _LastPos = 0;
        _moveTowardsNextPos = 1;
    }

    private void Update()
    {
        if(_positionList != null && _positionList.Count > 1)
            ChangeNextMovePos();
    }

    private void ChangeNextMovePos()
    {
        _timeIntoSegments += Time.deltaTime;
        if (_timeIntoSegments > _timePerSegments)
        {
            _LastPos = _moveTowardsNextPos;
            _moveTowardsNextPos++;
            if (_moveTowardsNextPos == _positionList.Count)
            {
                _moveTowardsNextPos = 0;
            }
            _timeIntoSegments -= _timePerSegments;
        }
        float param = _timeIntoSegments / _timePerSegments;
        transform.position = Vector3.Lerp(_positionList[_LastPos], _positionList[_moveTowardsNextPos], param);
    }
}
