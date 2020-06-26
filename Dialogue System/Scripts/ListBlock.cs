using System;
using System.Collections.Generic;
using System.Text;

public class ListBlock : Block
{
    bool isChoiceList;
    bool isRandom;
    bool isLooped;
    int defaultElem;

    List<string> blocksHeaders;
    List<Block> listedBlocks;

    bool isReady;
    int[] shuffledNodes;

    public ListBlock(bool isChoiceList, bool isRandom, bool isLooped, int defaultElem = 0)
    {
        this.isChoiceList = isChoiceList;
        this.isRandom = isRandom;
        this.isLooped = isLooped;
        this.defaultElem = defaultElem;

        blocksHeaders = new List<string>();
        listedBlocks = new List<Block>();

        isReady = false;
        shuffledNodes = null;
    }

    public void AddBlock(string header, Block block)
    {
        blocksHeaders.Add(header);
        listedBlocks.Add(block);
    }

    public override void SetSuccessor(Block block)
    {
        successor = block;

        foreach (Block b in listedBlocks)
        {
            if (b.successor == null)
            {
                b.SetSuccessor(block);
            }
        }
    }

    public override Block Enter(Dictionary<string, float> variables)
    {
        PrepareList();

        if (isChoiceList)
        {
            return this;
        }
        else
        {
            int idx = shuffledNodes[defaultElem++];

            if (idx == listedBlocks.Count)
            {
                if (isLooped)
                {
                    idx = 0;
                }
                else
                {
                    return null;
                }
            }

            return listedBlocks[idx];
        }
    }

    void PrepareList()
    {
        if (!isReady)
        {
            isReady = true;

            int n = listedBlocks.Count;

            shuffledNodes = new int[n];
            for (int i = 0; i < n; ++i)
            {
                shuffledNodes[i] = i;
            }

            if (isRandom)
            {
                Random rand = new Random();

                for (int i = 0; i < n; ++i)
                {
                    int a = rand.Next(0, n);
                    int b = rand.Next(0, n);

                    int tmp = shuffledNodes[a];
                    shuffledNodes[a] = shuffledNodes[b];
                    shuffledNodes[b] = tmp;
                }
            }
        }
    }

    public override string ToString()
    {
        if (isChoiceList)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < blocksHeaders.Count; ++i)
            {
                builder.Append(blocksHeaders[i]);
                builder.Append(',');
                builder.Append(listedBlocks[i]);
                builder.Append('|');
            }

            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }
        else
        {
            return base.ToString();
        }
    }

    public Block Select(Dictionary<string, float> variables, int idx = -1)
    {
        if (idx == -1) idx = defaultElem;

        return listedBlocks[shuffledNodes[idx]].Enter(variables);
    }
}
