using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystem : MonoBehaviour
{
    [SerializeField] GameObject turtle;

    [SerializeField] char axiom = 'F';
    [SerializeField] int iterations = 2;

    [SerializeField] float length = 10.0f;
    [SerializeField] Color line_color = Color.red;

    private List<char> current_sentence;
    private List<char> next_sentence;

    private List<Vector3> reset_positions;
    private List<Quaternion> reset_rotations;

    private List<char> rule;
    private string rule1 = "FF+[+F-F-F]-[-F+F+F]";

	// Use this for initialization
	void Start ()
    {
        // Initialisation...
        current_sentence = new List<char>();
        next_sentence    = new List<char>();

        reset_positions = new List<Vector3>();
        reset_rotations = new List<Quaternion>();

        rule = new List<char>();

        foreach(char character in rule1)
        {
            rule.Add(character);
        }

        current_sentence.Add(axiom);

        // loop through iterations
        for (int i = 0; i < iterations; i++)
        {
            UpdateSentence();

            current_sentence.Clear();

            foreach (char character in next_sentence)
            {
                current_sentence.Add(character);
            }

            DrawSentence(current_sentence);

            next_sentence.Clear();
        }

        Destroy(turtle);
    }


    private void UpdateSentence()
    {
        for (int i = 0; i < current_sentence.Count; i++)
        {
            SetChars(current_sentence[i]);
        }
    }


    private void SetChars(char _char)
    {
        // Rule 1 *** A Becomes AB ***
        /*if (_char == 'A')
        {
            next_sentence.Add('A');
            next_sentence.Add('B');
            next_sentence.Add('A');
        }

        // Rule 2 *** B becomes A ***
        if (_char == 'B')
        {
            next_sentence.Add('B');
            next_sentence.Add('B');
            next_sentence.Add('B');
        }*/

        if (_char == 'F')
        {
            foreach (char character in rule)
            {
                next_sentence.Add(character);
            }
            return;
        }

        // if the current char doesnt match a rule, 
        // simply add the character
        else
            next_sentence.Add(_char);
    }


    private void DrawSentence(List<char> _sentence)
    {
        length *= 0.5F;

        foreach (char character in _sentence)
        {
            if (character == 'F')
            {
                // Move Forward, draw line

                var start = turtle.transform.position;
                turtle.transform.Translate(Vector3.forward * length);
                var end = turtle.transform.position;

                DrawLine(start, end);

                Debug.Log("Drawing Line Forward");
            }

            else if (character == '+')
            {
                // Turn Right

                turtle.transform.Rotate(transform.up, 25.0f);

                Debug.Log("Turning Right");
            }

            else if (character == '-')
            {
                // Turn Left

                turtle.transform.Rotate(transform.up, -25.0f);

                Debug.Log("Turning Left");
            }


            else if (character == '[')
            {
                // Push

                reset_positions.Add(turtle.transform.position);
                reset_rotations.Add(turtle.transform.rotation);

                Debug.Log("Setting Reset Pos");
            }

            else if (character == ']')
            {
                // Pop

                turtle.transform.position = reset_positions[reset_positions.Count - 1];
                turtle.transform.rotation = reset_rotations[reset_rotations.Count - 1];

                reset_positions.RemoveAt(reset_positions.Count - 1);
                reset_rotations.RemoveAt(reset_rotations.Count - 1);

                Debug.Log("Reverting Position");
            }
        }
    }


    private void DrawLine(Vector3 _start, Vector3 _end)
    {
        GameObject line = new GameObject();
        line.transform.position = _start;
        line.AddComponent<LineRenderer>();

        LineRenderer lr = line.GetComponent<LineRenderer>();

        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));

        lr.startColor = line_color;
        lr.startWidth = 0.1f;
        lr.SetPosition(0, _start);
        lr.SetPosition(1, _end);
    }
}
