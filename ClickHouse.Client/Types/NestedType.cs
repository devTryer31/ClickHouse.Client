﻿using System;
using System.Linq;
using ClickHouse.Client.Types.Grammar;

namespace ClickHouse.Client.Types;

internal class NestedType : TupleType
{
    public override string Name => "Nested";

    public override Type FrameworkType => base.FrameworkType.MakeArrayType();

    public override ParameterizedType Parse(SyntaxTreeNode node, Func<SyntaxTreeNode, ClickHouseType> parseClickHouseTypeFunc, TypeSettings settings)
    {
        return new NestedType
        {
            UnderlyingTypes = node.ChildNodes.Select(ClearFieldName).Select(parseClickHouseTypeFunc).ToArray(),
        };
    }

    // Try to determine if something which is inside a Nested column is a name-type pair
    // (param_id UInt8) or a more complex structure (another parameterized type)
    // We do not currently support multi-word types
    private static SyntaxTreeNode ClearFieldName(SyntaxTreeNode node)
    {
        if (node.ChildNodes.Count > 0)
            return node;

        var name = node.Value;

        var lastSpaceIndex = name.LastIndexOf(' ');
        return lastSpaceIndex > 0 ? new SyntaxTreeNode { Value = name.Substring(lastSpaceIndex + 1) } : node;
    }
}
