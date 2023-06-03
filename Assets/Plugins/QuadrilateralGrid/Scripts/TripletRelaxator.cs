using UnityEngine;

public static class TripletRelaxator
{
    public static void RelaxTriplet(int direction,
        HexagonRelaxationData central,
        HexagonRelaxationData directionNeighbor,
        HexagonRelaxationData directionMinusOneNeighbor)
    {
        Vector3[] centralOffsets;
        int[] centralCounts;
        Vector3[] directionOffsets;
        int[] directionCounts;
        Vector3[] directionMinusOneOffsets;
        int[] directionMinusOneCounts;

        int[] directionArray = new int[6];
        directionArray[0] = direction;
        for (int i = 1; i < 6; ++i)
        {
            directionArray[i] = (direction + i) % 6;
        }

        bool available1 = central.GetOffsetsInDirectionFirst(directionArray[0], out centralOffsets, out centralCounts);
        bool available2 = directionNeighbor.GetOffsetsInDirectionFirst(directionArray[4], out directionOffsets, out directionCounts);
        bool available3 = directionMinusOneNeighbor.GetOffsetsInDirectionFirst(directionArray[2], out directionMinusOneOffsets, out directionMinusOneCounts);

        if (available1 && available2 && available3)
        {
            AddNeighborsOffsetsAndCounts(
                centralOffsets,
                centralCounts,
                directionOffsets,
                directionCounts,
                directionMinusOneOffsets,
                directionMinusOneCounts,
                directionArray);
            central.ApplyOffsetInDirection(directionArray[0], centralOffsets, centralCounts);
            directionNeighbor.ApplyOffsetInDirection(directionArray[4], directionOffsets, directionCounts);
            directionMinusOneNeighbor.ApplyOffsetInDirection(directionArray[2], directionMinusOneOffsets, directionMinusOneCounts);
            for (int i = 1; i < RelaxationProperties.iterationsCount; ++i)
            {
                central.GetOffsetsInDirection(directionArray[0], out centralOffsets, out centralCounts);
                directionNeighbor.GetOffsetsInDirection(directionArray[4], out directionOffsets, out directionCounts);
                directionMinusOneNeighbor.GetOffsetsInDirection(directionArray[2], out directionMinusOneOffsets, out directionMinusOneCounts);
                AddNeighborsOffsets(
                    centralOffsets,
                    directionOffsets,
                    directionMinusOneOffsets,
                    directionArray);
                central.ApplyOffsetInDirection(directionArray[0], centralOffsets, centralCounts);
                directionNeighbor.ApplyOffsetInDirection(directionArray[4], directionOffsets, directionCounts);
                directionMinusOneNeighbor.ApplyOffsetInDirection(directionArray[2], directionMinusOneOffsets, directionMinusOneCounts);
            }

            central.SetRelaxedInDirection(directionArray[0]);
            directionNeighbor.SetRelaxedInDirection(directionArray[4]);
            directionMinusOneNeighbor.SetRelaxedInDirection(directionArray[2]);
        }
    }

    private static void AddNeighborsOffsetsAndCounts(
        Vector3[] centralOffsets,
        int[] centralCounts,
        Vector3[] directionOffsets,
        int[] directionCounts,
        Vector3[] directionMinusOneOffsets,
        int[] directionMinusOneCounts,
        int[] directionArray)
    {
        int ind1 = UniversalHexagon.pointIndexArray[directionArray[2]];
        int ind2 = UniversalHexagon.pointIndexArray[directionArray[0]];
        int ind3 = UniversalHexagon.pointIndexArray[directionArray[4]];
        centralOffsets[ind1] += directionOffsets[ind2] + directionMinusOneOffsets[ind3];
        directionOffsets[ind2] = centralOffsets[ind1];
        directionMinusOneOffsets[ind3] = centralOffsets[ind1];
        centralCounts[ind1] += directionCounts[ind2] + directionMinusOneCounts[ind3];
        directionCounts[ind2] = centralCounts[ind1];
        directionMinusOneCounts[ind3] = centralCounts[ind1];

        ind1 = UniversalHexagon.pointIndexArray[directionArray[1]];
        ind2 = UniversalHexagon.pointIndexArray[directionArray[5]];
        ind3 = UniversalHexagon.pointIndexArray[directionArray[3]];
        centralOffsets[ind1] += directionMinusOneOffsets[ind2];
        directionMinusOneOffsets[ind2] = centralOffsets[ind1];
        centralCounts[ind1] += directionMinusOneCounts[ind2];
        directionMinusOneCounts[ind2] = centralCounts[ind1];

        directionOffsets[ind2] += centralOffsets[ind3];
        centralOffsets[ind3] = directionOffsets[ind2];
        directionCounts[ind2] += centralCounts[ind3];
        centralCounts[ind3] = directionCounts[ind2];

        directionMinusOneOffsets[ind3] += directionOffsets[ind1];
        directionOffsets[ind1] = directionMinusOneOffsets[ind3];
        directionMinusOneCounts[ind3] += directionCounts[ind1];
        directionCounts[ind1] = directionMinusOneCounts[ind3];

        ind1 = UniversalHexagon.edgeIndexArray[directionArray[1]];
        ind2 = UniversalHexagon.edgeIndexOppositeArray[directionArray[4]];
        centralOffsets[ind1] += directionMinusOneOffsets[ind2];
        directionMinusOneOffsets[ind2] = centralOffsets[ind1];
        centralCounts[ind1] += directionMinusOneCounts[ind2];
        directionMinusOneCounts[ind2] = centralCounts[ind1];

        ind1 = UniversalHexagon.edgeIndexArray[directionArray[5]];
        ind2 = UniversalHexagon.edgeIndexOppositeArray[directionArray[2]];
        directionOffsets[ind1] += centralOffsets[ind2];
        centralOffsets[ind2] = directionOffsets[ind1];
        directionCounts[ind1] += centralCounts[ind2];
        centralCounts[ind2] = directionCounts[ind1];

        ind1 = UniversalHexagon.edgeIndexArray[directionArray[3]];
        ind2 = UniversalHexagon.edgeIndexOppositeArray[directionArray[0]];
        directionMinusOneOffsets[ind1] += directionOffsets[ind2];
        directionOffsets[ind2] = directionMinusOneOffsets[ind1];
        directionMinusOneCounts[ind1] += directionCounts[ind2];
        directionCounts[ind2] = directionMinusOneCounts[ind1];

        for (int i = 1; i < UniversalHexagon.hexagonRadius; ++i)
        {
            ind3 = i * 3;
            ind1 = UniversalHexagon.pointIndexArray[directionArray[1]] + i;
            ind2 = UniversalHexagon.pointIndexOppositeArray[directionArray[4]] - i;
            centralOffsets[ind1] += directionMinusOneOffsets[ind2];
            directionMinusOneOffsets[ind2] = centralOffsets[ind1];
            centralCounts[ind1] += directionMinusOneCounts[ind2];
            directionMinusOneCounts[ind2] = centralCounts[ind1];
            ind1 = UniversalHexagon.edgeIndexArray[directionArray[1]] + ind3;
            ind2 = UniversalHexagon.edgeIndexOppositeArray[directionArray[4]] - ind3;
            centralOffsets[ind1] += directionMinusOneOffsets[ind2];
            directionMinusOneOffsets[ind2] = centralOffsets[ind1];
            centralCounts[ind1] += directionMinusOneCounts[ind2];
            directionMinusOneCounts[ind2] = centralCounts[ind1];

            ind1 = UniversalHexagon.pointIndexArray[directionArray[5]] + i;
            ind2 = UniversalHexagon.pointIndexOppositeArray[directionArray[2]] - i;
            directionOffsets[ind1] += centralOffsets[ind2];
            centralOffsets[ind2] = directionOffsets[ind1];
            directionCounts[ind1] += centralCounts[ind2];
            centralCounts[ind2] = directionCounts[ind1];
            ind1 = UniversalHexagon.edgeIndexArray[directionArray[5]] + ind3;
            ind2 = UniversalHexagon.edgeIndexOppositeArray[directionArray[2]] - ind3;
            directionOffsets[ind1] += centralOffsets[ind2];
            centralOffsets[ind2] = directionOffsets[ind1];
            directionCounts[ind1] += centralCounts[ind2];
            centralCounts[ind2] = directionCounts[ind1];

            ind1 = UniversalHexagon.pointIndexArray[directionArray[3]] + i;
            ind2 = UniversalHexagon.pointIndexOppositeArray[directionArray[0]] - i;
            directionMinusOneOffsets[ind1] += directionOffsets[ind2];
            directionOffsets[ind2] = directionMinusOneOffsets[ind1];
            directionMinusOneCounts[ind1] += directionCounts[ind2];
            directionCounts[ind2] = directionMinusOneCounts[ind1];
            ind1 = UniversalHexagon.edgeIndexArray[directionArray[3]] + ind3;
            ind2 = UniversalHexagon.edgeIndexOppositeArray[directionArray[0]] - ind3;
            directionMinusOneOffsets[ind1] += directionOffsets[ind2];
            directionOffsets[ind2] = directionMinusOneOffsets[ind1];
            directionMinusOneCounts[ind1] += directionCounts[ind2];
            directionCounts[ind2] = directionMinusOneCounts[ind1];
        }
    }

    private static void AddNeighborsOffsets(
        Vector3[] centralOffsets,
        Vector3[] directionOffsets,
        Vector3[] directionMinusOneOffsets,
        int[] directionArray)
    {
        int ind1 = UniversalHexagon.pointIndexArray[directionArray[2]];
        int ind2 = UniversalHexagon.pointIndexArray[directionArray[0]];
        int ind3 = UniversalHexagon.pointIndexArray[directionArray[4]];
        centralOffsets[ind1] += directionOffsets[ind2] + directionMinusOneOffsets[ind3];
        directionOffsets[ind2] = centralOffsets[ind1];
        directionMinusOneOffsets[ind3] = centralOffsets[ind1];

        ind1 = UniversalHexagon.pointIndexArray[directionArray[1]];
        ind2 = UniversalHexagon.pointIndexArray[directionArray[5]];
        ind3 = UniversalHexagon.pointIndexArray[directionArray[3]];
        centralOffsets[ind1] += directionMinusOneOffsets[ind2];
        directionMinusOneOffsets[ind2] = centralOffsets[ind1];

        directionOffsets[ind2] += centralOffsets[ind3];
        centralOffsets[ind3] = directionOffsets[ind2];

        directionMinusOneOffsets[ind3] += directionOffsets[ind1];
        directionOffsets[ind1] = directionMinusOneOffsets[ind3];

        ind1 = UniversalHexagon.edgeIndexArray[directionArray[1]];
        ind2 = UniversalHexagon.edgeIndexOppositeArray[directionArray[4]];
        centralOffsets[ind1] += directionMinusOneOffsets[ind2];
        directionMinusOneOffsets[ind2] = centralOffsets[ind1];

        ind1 = UniversalHexagon.edgeIndexArray[directionArray[5]];
        ind2 = UniversalHexagon.edgeIndexOppositeArray[directionArray[2]];
        directionOffsets[ind1] += centralOffsets[ind2];
        centralOffsets[ind2] = directionOffsets[ind1];

        ind1 = UniversalHexagon.edgeIndexArray[directionArray[3]];
        ind2 = UniversalHexagon.edgeIndexOppositeArray[directionArray[0]];
        directionMinusOneOffsets[ind1] += directionOffsets[ind2];
        directionOffsets[ind2] = directionMinusOneOffsets[ind1];

        for (int i = 1; i < UniversalHexagon.hexagonRadius; ++i)
        {
            ind3 = i * 3;
            ind1 = UniversalHexagon.pointIndexArray[directionArray[1]] + i;
            ind2 = UniversalHexagon.pointIndexOppositeArray[directionArray[4]] - i;
            centralOffsets[ind1] += directionMinusOneOffsets[ind2];
            directionMinusOneOffsets[ind2] = centralOffsets[ind1];
            ind1 = UniversalHexagon.edgeIndexArray[directionArray[1]] + ind3;
            ind2 = UniversalHexagon.edgeIndexOppositeArray[directionArray[4]] - ind3;
            centralOffsets[ind1] += directionMinusOneOffsets[ind2];
            directionMinusOneOffsets[ind2] = centralOffsets[ind1];

            ind1 = UniversalHexagon.pointIndexArray[directionArray[5]] + i;
            ind2 = UniversalHexagon.pointIndexOppositeArray[directionArray[2]] - i;
            directionOffsets[ind1] += centralOffsets[ind2];
            centralOffsets[ind2] = directionOffsets[ind1];
            ind1 = UniversalHexagon.edgeIndexArray[directionArray[5]] + ind3;
            ind2 = UniversalHexagon.edgeIndexOppositeArray[directionArray[2]] - ind3;
            directionOffsets[ind1] += centralOffsets[ind2];
            centralOffsets[ind2] = directionOffsets[ind1];

            ind1 = UniversalHexagon.pointIndexArray[directionArray[3]] + i;
            ind2 = UniversalHexagon.pointIndexOppositeArray[directionArray[0]] - i;
            directionMinusOneOffsets[ind1] += directionOffsets[ind2];
            directionOffsets[ind2] = directionMinusOneOffsets[ind1];
            ind1 = UniversalHexagon.edgeIndexArray[directionArray[3]] + ind3;
            ind2 = UniversalHexagon.edgeIndexOppositeArray[directionArray[0]] - ind3;
            directionMinusOneOffsets[ind1] += directionOffsets[ind2];
            directionOffsets[ind2] = directionMinusOneOffsets[ind1];
        }
    }
}