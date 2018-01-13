public partial class Spawner
{
    private void Clean()
    {
        //ToDo - clean anim? like cuphead or w/e

        if (objectsOnScene.Count < 1)
            return;

        foreach (var go in objectsOnScene)
            Destroy(go);
    }
}
