using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHandler {
	public Resource Gold { get; } = new Resource(nameof(Gold));
	public Resource Health { get; } = new Resource(nameof(Health));
	public Resource Brawn { get; } = new Resource(nameof(Brawn));
	public Resource Coal { get; } = new Resource(nameof(Coal));
}
