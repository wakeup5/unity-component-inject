using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Waker.Injection;

[Category("Query")]
public class QueryTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void IsMatchTest()
    {
        var root = new GameObject("Root");
        var grandParent = new GameObject("GrandParent #grand-parent");
        var parent = new GameObject("Parent #parent");
        var current = new GameObject("Current #current");
        var child = new GameObject("Child #child");
        var grandChild = new GameObject("GranndChild #grand-child");

        grandParent.transform.SetParent(root.transform);
        parent.transform.SetParent(grandParent.transform);
        current.transform.SetParent(parent.transform);
        child.transform.SetParent(current.transform);
        grandChild.transform.SetParent(child.transform);

        Query query = new Query("#parent > #current");

        Assert.IsTrue(query.IsMatch(root.transform, current.transform));
        Assert.IsFalse(query.IsMatch(root.transform, parent.transform));
        Assert.IsFalse(query.IsMatch(root.transform, child.transform));

        Query query2 = new Query("#current");

        Assert.IsTrue(query2.IsMatch(root.transform, current.transform));
        Assert.IsFalse(query2.IsMatch(root.transform, parent.transform));
        Assert.IsFalse(query2.IsMatch(root.transform, child.transform));

        Query query3 = new Query("#grand-parent > #current");

        Assert.IsTrue(query3.IsMatch(root.transform, current.transform));
        Assert.IsFalse(query3.IsMatch(root.transform, parent.transform));
        Assert.IsFalse(query3.IsMatch(root.transform, child.transform));

        Query query1 = new Query("#no > #current");

        Assert.IsFalse(query1.IsMatch(root.transform, current.transform));
        Assert.IsFalse(query1.IsMatch(root.transform, parent.transform));
        Assert.IsFalse(query1.IsMatch(root.transform, child.transform));
    }
}
