using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Enemy))]
public class SlopeGeneratorInspector : Editor {
    private Enemy enemy;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        enemy = target as Enemy;

        //if the player clicks the "Generate edge triggers" button
        //(also creates that button)
        if(GUILayout.Button("Generate Edge Triggers")) {

            //start undo event
            Undo.SetCurrentGroupName("Generate Edge Triggers");

            //delete any old edge triggers
            DestroyEdgeTriggers();

            //generate new ones
            Bounds[] ObstructionShapes = enemy.GenerateObstructionShapes();
            //foreach( Bounds b in ObstructionShapes) {

            GameObject parent = new GameObject();
            parent.transform.SetParent(enemy.transform);
            parent.transform.localPosition = Vector3.zero;
            parent.transform.localScale = Vector3.one;
            parent.transform.localRotation = Quaternion.identity;
            parent.name = "Edge Triggers";
            enemy.EdgeTriggerFolder = parent;

            enemy.EdgeTriggers = new EnemyEdgeChecker[5];
            for (int n = 0; n < 5; n++) {
                Bounds b = ObstructionShapes[n];

                //make the object and move it to the appropriate edge
                GameObject go = new GameObject();
                go.transform.SetParent(parent.transform);
                go.transform.localPosition = b.center;
                if(n == 4) {
                    go.name = "Direct-Below Trigger";
                }
                else {
                    go.name = ((Direction)n).ToString() + " Edge Trigger";
                }
                go.layer = LAYER.EDGE_TRIGGER;

                //give it a box collider and set it to the appropriate shape
                BoxCollider2D col = go.AddComponent<BoxCollider2D>();
                col.isTrigger = true;
                col.size = b.size;

                //give it an EnemyEdgeChecker and set it up
                EnemyEdgeChecker checker = go.AddComponent<EnemyEdgeChecker>();
                checker.enemy = enemy;

                //add the checker to the Enemy's edgeTriggers
                enemy.EdgeTriggers[n] = checker;

                //register to the undo event
                Undo.RegisterCreatedObjectUndo(go, "Make Edge");
            }

            //end undo event
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

    }

    //destroy all edge triggers if they exist
    void DestroyEdgeTriggers() {
        enemy = target as Enemy;
        if(enemy.EdgeTriggerFolder != null) {
            Undo.DestroyObjectImmediate(enemy.EdgeTriggerFolder);
        }

        for (int n = 0; n < enemy.transform.childCount; n++) {
            if (enemy.transform.GetChild(n).name == "Edge Triggers") {
                Debug.Log(enemy.transform.GetChild(n).name);
                Undo.DestroyObjectImmediate(enemy.transform.GetChild(n).gameObject);
            }
        }

        EditorUtility.SetDirty(enemy);
    }

}