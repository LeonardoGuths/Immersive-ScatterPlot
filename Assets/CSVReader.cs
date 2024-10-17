using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CSVReader
{
    // Função para ler o CSV com cabeçalhos e retornar uma lista de dicionários
    public static List<Dictionary<string, object>> Read(TextAsset csv)
    {
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

        if (csv == null || string.IsNullOrEmpty(csv.text))
        {
            return rows;
        }

        // Separa as linhas
        string[] lines = csv.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // Lê o cabeçalho (primeira linha)
        string[] headers = lines[0].Split(new char[] { ',' }, StringSplitOptions.None);

        // Processa cada linha de dados
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(new char[] { ',' }, StringSplitOptions.None);
            Dictionary<string, object> row = new Dictionary<string, object>();

            // Associa cada valor com o nome da coluna correspondente
            for (int j = 0; j < headers.Length; j++)
            {
                // Garante que os valores sejam armazenados corretamente
                row[headers[j]] = values[j];
            }

            rows.Add(row);
        }

        return rows;
    }
}
