using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class DataPlotter : MonoBehaviour
{
    [Header("Arquivo CSV do Dataset")]

    public TextAsset inputfile;
    private List<Dictionary<string, object>> pointList;
    [Header("Configurações de Colunas")]

    public int columnX = 1;  // 'Alcohol'
    public int columnY = 2;  // 'MalicAcid'
    public int columnZ = 5;  // 'Magnesium'

    private string xName;
    private string yName;
    private string zName;

    public Color colorAxisX = Color.red; // Cor para o eixo X
    public Color colorAxisY = Color.green; // Cor para o eixo Y
    public Color colorAxisZ = Color.blue; // Cor para o eixo Z

    [Header("Escala do Plot")]

    public float plotScale = 10;

    // O prefab para os pontos de dados que serão instanciados
    [Header("Prefabs")]

    public GameObject PointPrefab;
 
    // Objeto que conterá os prefabs instanciados na hierarquia
    public GameObject PointHolder;

	public GameObject axisPrefab;
	private GameObject Xaxis, Yaxis, Zaxis;

    private List<string> columnList;

    // Variáveis para armazenar os valores mínimo e máximo de cada coluna
    private float minX, maxX, minY, maxY, minZ, maxZ;

    private int prevColumnX, prevColumnY, prevColumnZ;
private float prevPlotScale;

void Start()
{
    if (inputfile != null)
    {
        pointList = CSVReader.Read(inputfile);

        if (pointList != null && pointList.Count > 0)
        {
            columnList = new List<string>(pointList[0].Keys);

            xName = columnList[columnX];
            yName = columnList[columnY];
            zName = columnList[columnZ];

            // Instanciando os eixos
            Xaxis = Instantiate(axisPrefab, new Vector3(0,3.3f,0), Quaternion.Euler(0,0,0));
            Yaxis = Instantiate(axisPrefab, new Vector3(-3.3f,0,0), Quaternion.Euler(0,0,90));
            Zaxis = Instantiate(axisPrefab, new Vector3(0,3.3f,0), Quaternion.Euler(0,-90,0));



            UpdateTexts(xName, yName, zName);

            // Encontrar os valores mínimo e máximo das colunas
            FindMinMaxValues();

            CreatePlot();

            // Armazena os valores iniciais
            prevColumnX = columnX;
            prevColumnY = columnY;
            prevColumnZ = columnZ;
            prevPlotScale = plotScale;
        }
        else
        {
            Debug.LogError("O CSV está vazio ou não foi lido corretamente.");
        }
    }
    else
    {
        Debug.LogError("Arquivo CSV não atribuído!");
    }
}

void Update()
{
    // Verifica se houve mudança nas colunas ou no plotScale
    if (columnX != prevColumnX || columnY != prevColumnY || columnZ != prevColumnZ || plotScale != prevPlotScale)
    {
        // Atualiza o gráfico com os novos valores
        UpdatePlot();
    }

    UpdateTexts(xName, yName, zName);
}

void UpdatePlot()
{
    // Limpa os pontos anteriores
    foreach (Transform child in PointHolder.transform)
    {
        Destroy(child.gameObject);
    }

    xName = columnList[columnX];
    yName = columnList[columnY];
    zName = columnList[columnZ];

    // Atualiza os textos dos eixos
    UpdateTexts(xName, yName, zName);

    // Recria o gráfico
    CreatePlot();

    // Armazena os novos valores
    prevColumnX = columnX;
    prevColumnY = columnY;
    prevColumnZ = columnZ;
    prevPlotScale = plotScale;
}

void CreatePlot()
{
    for (var i = 0; i < pointList.Count; i++)
    {
        try
        {
            // Pega os valores brutos das colunas
            string xValue = pointList[i][xName].ToString().Trim();
            string yValue = pointList[i][yName].ToString().Trim();
            string zValue = pointList[i][zName].ToString().Trim();

            xValue = FixDecimalFormat(xValue);
            yValue = FixDecimalFormat(yValue);
            zValue = FixDecimalFormat(zValue);

            float xRaw = float.Parse(xValue, CultureInfo.InvariantCulture);
            float yRaw = float.Parse(yValue, CultureInfo.InvariantCulture);
            float zRaw = float.Parse(zValue, CultureInfo.InvariantCulture);

            // Normalizar os valores
            float x = NormalizeValue(xRaw, minX, maxX);
            float y = NormalizeValue(yRaw, minY, maxY);
            float z = NormalizeValue(zRaw, minZ, maxZ);

            // Instancia como variável GameObject para que possa ser manipulada dentro do loop
            GameObject dataPoint = Instantiate(PointPrefab, new Vector3(x, y, z) * plotScale, Quaternion.identity);
                                
            // Faz filho do objeto PointHolder, para manter os pontos dentro do contêiner na hierarquia
            dataPoint.transform.parent = PointHolder.transform;

            // Atribui os valores originais ao dataPointName
            string dataPointName = 
                "X: " + pointList[i][xName] + "\nY: " + pointList[i][yName] + "\nZ: " + pointList[i][zName];

            dataPoint.GetComponentInChildren<TextMesh>().text = dataPointName;

            // Atribui nome ao prefab
            dataPoint.transform.name = dataPointName;

            // Obtém a cor do material e define como uma nova cor RGB que definimos
            dataPoint.GetComponent<Renderer>().material.color = new Color(x, y, z, 1.0f);
            dataPoint.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(x, y, z, 1.0f));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro de formatação na linha {i + 1}: {e.Message}");
        }
    }
}


    private void ApplyColorToAxis(GameObject axis, Color color)
    {
        Renderer[] renderers = axis.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color;
        }
    }

    // Função para encontrar os valores mínimo e máximo de cada coluna (X, Y, Z)
    void UpdateTexts(string xName, string yName, string zName)
    {
        var textosDoEixoX = Xaxis.GetComponentsInChildren<TextMesh>();
        foreach (var textodoeixo in textosDoEixoX)
        {
            if (textodoeixo.name == "TextoDoEixo")
            {
                RotateTexts(textodoeixo.transform.gameObject);
                textodoeixo.text = xName;  // Atualiza com o nome do eixo X
            }
        }

        var textosDoEixoY = Yaxis.GetComponentsInChildren<TextMesh>();
        foreach (var textodoeixoY in textosDoEixoY)
        {
            if (textodoeixoY.name == "TextoDoEixo")
            {
                RotateTexts(textodoeixoY.transform.gameObject);
                textodoeixoY.text = yName;  // Atualiza com o nome do eixo Y
            }
        }

        var textosDoEixoZ = Zaxis.GetComponentsInChildren<TextMesh>();
        foreach (var textodoeixoZ in textosDoEixoZ)
        {
            if (textodoeixoZ.name == "TextoDoEixo")
            {
                RotateTexts(textodoeixoZ.transform.gameObject);
                textodoeixoZ.text = zName;  // Atualiza com o nome do eixo Z
            }
        }

        ApplyColorToAxis(Xaxis, colorAxisX);
        ApplyColorToAxis(Yaxis, colorAxisY);
        ApplyColorToAxis(Zaxis, colorAxisZ);
    }

	private void RotateTexts (GameObject textObject){
		textObject.transform.LookAt(Camera.main.transform);
		textObject.transform.rotation = Quaternion.LookRotation(textObject.transform.position - Camera.main.transform.position);
	}
    void FindMinMaxValues()
    {
        minX = float.MaxValue; maxX = float.MinValue;
        minY = float.MaxValue; maxY = float.MinValue;
        minZ = float.MaxValue; maxZ = float.MinValue;

        foreach (var row in pointList)
        {
            float xRaw = float.Parse(FixDecimalFormat(row[xName].ToString()), CultureInfo.InvariantCulture);
            float yRaw = float.Parse(FixDecimalFormat(row[yName].ToString()), CultureInfo.InvariantCulture);
            float zRaw = float.Parse(FixDecimalFormat(row[zName].ToString()), CultureInfo.InvariantCulture);

            if (xRaw < minX) minX = xRaw;
            if (xRaw > maxX) maxX = xRaw;

            if (yRaw < minY) minY = yRaw;
            if (yRaw > maxY) maxY = yRaw;

            if (zRaw < minZ) minZ = zRaw;
            if (zRaw > maxZ) maxZ = zRaw;
        }

        Debug.Log($"Min X: {minX}, Max X: {maxX}");
        Debug.Log($"Min Y: {minY}, Max Y: {maxY}");
        Debug.Log($"Min Z: {minZ}, Max Z: {maxZ}");
    }

    // Função para normalizar os valores usando min-max normalization
    // Função para normalizar os valores usando min-max normalization para o intervalo [-1, 1]
    float NormalizeValue(float value, float minValue, float maxValue)
    {
        return  ((value - minValue) / (maxValue - minValue));
    }

    // float NormalizeNegAndPosValue(float value, float minValue, float maxValue)
    // {
    //     return  2*((value - minValue) / (maxValue - minValue))-1;
    // }


    // Função para corrigir o formato decimal
    string FixDecimalFormat(string value)
    {
        if (value.StartsWith("."))
        {
            return "0" + value;
        }
        return value;
    }
}
