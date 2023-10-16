using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TrainMover : MonoBehaviour {
	[SerializeField] Transform train;
	[SerializeField] Transform exitEndPoint;
	[SerializeField] Transform enterStartPoint;
	[SerializeField] ParticleSystem smokeParticles;
	[Space]
	[SerializeField] float maxSpeedExit;
	[SerializeField] float secondsToMaxExit;
	[SerializeField] float delayExit;
	[Space]
	[SerializeField] float maxSpeedEnter;
	[SerializeField] float secondsToMaxEnter;
	[SerializeField] float delayEnter;
	[Space]
	[SerializeField] float amount;
	[SerializeField] float cycleLength;

	TransformFloater[] floaters;
	TransformRotator[] rotators;
	TransformScaler[] scalers;
	Vector3 startPos;

	void Start() {
		exitEndPoint.localPosition *= AspectRatioInstance.Instance.GetWidthMultiplier();
		enterStartPoint.localPosition *= AspectRatioInstance.Instance.GetWidthMultiplier();
		rotators = transform.GetComponentsInChildren<TransformRotator>();
		scalers = transform.GetComponentsInChildren<TransformScaler>();
		floaters = transform.GetComponentsInChildren<TransformFloater>();
		startPos = train.position;
	}

	public void SetFloating(bool floating) {
		if (floating) foreach (var floater in floaters) floater.SetFloating(amount, cycleLength);
		else foreach (var floater in floaters) floater.StopFloating();
	}

	public async Task TrainEnter(bool keepRotating) {
		smokeParticles.Clear();
		await ChangeTrainPosition(enterStartPoint.position, startPos, maxSpeedEnter, secondsToMaxEnter, delayEnter, keepRotating);
	}

	public async Task TrainExit() => await ChangeTrainPosition(startPos, exitEndPoint.position, maxSpeedExit, secondsToMaxExit, delayExit, true);

	public async Task StopTrain() {
		foreach (var rotator in rotators) rotator.StopRotating(maxSpeedEnter, secondsToMaxEnter);
		await Awaiters.Seconds(secondsToMaxEnter);
		smokeParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		foreach (var scaler in scalers) scaler.SetScale(0, secondsToMaxEnter);
	}

	public async Task ChangeTrainPosition(Vector3 start, Vector3 target, float maxSpeed, float secondsToMax, float delay, bool keepRotating) {
		train.position = start;
		var speed = 0f;
		float targetSpeed = maxSpeed;
		bool stopping = false;
		smokeParticles.Play();
		var velocityModule = smokeParticles.velocityOverLifetime;
		velocityModule.x = new ParticleSystem.MinMaxCurve(0);
		await Awaiters.Seconds(delay);

		foreach (var rotator in rotators) rotator.StartRotating(maxSpeed, secondsToMax);
		foreach (var scaler in scalers) scaler.SetScale(1, secondsToMaxEnter);

		while (Vector3.Distance(train.position, target) > Mathf.Epsilon) {
			if (!keepRotating && !stopping && Vector3.Distance(train.position, target) <= speed * (speed/maxSpeed*secondsToMax) / 2f) {
				stopping = true;
				targetSpeed = 0;
				_ = StopTrain();
			}
			speed = Mathf.MoveTowards(speed, targetSpeed, maxSpeed * Time.deltaTime / secondsToMax);
			train.position = Vector3.MoveTowards(train.position, target, speed * Time.deltaTime);
			if (stopping && speed <= 0) break;
			await Awaiters.NextFrame;
		}

		if (keepRotating) {
			velocityModule.x = new ParticleSystem.MinMaxCurve(-maxSpeed);
		}

		train.position = target;
	}
}
