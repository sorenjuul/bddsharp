<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="text"/>
	<xsl:key name="rank-by-level" match="node" use="level" />
	<xsl:template match="document">
		digraph <xsl:value-of select="./@name"/> {
		size="<xsl:value-of select="./@size"/>"
		{node[shape=box]; 0; 1;}
		<xsl:for-each select="node[count(. | key('rank-by-level', level)[1]) = 1]">
			<xsl:sort select="level" />
			{rank=same;<xsl:for-each select="key('rank-by-level', level)">
				<xsl:sort select="u" />
				<xsl:value-of select="u" />
				<xsl:text> </xsl:text>
			</xsl:for-each>}
		</xsl:for-each>
		<xsl:apply-templates mode="label"/>
		<xsl:apply-templates mode="edge"/>}
	</xsl:template>

	<xsl:template match="document/node" mode="label">
		<xsl:value-of select="u"/>
		<xsl:text disable-output-escaping="no"> [label = "x</xsl:text>
		<xsl:value-of select="level"/>
		<xsl:text disable-output-escaping="no">"];</xsl:text>
	</xsl:template>

	<xsl:template match="document/node" mode="edge">
		<xsl:value-of select="u"/><xsl:text disable-output-escaping="yes"> -> </xsl:text><xsl:value-of select="low"/> [style = dotted];
		<xsl:value-of select="u"/><xsl:text disable-output-escaping="yes"> -> </xsl:text><xsl:value-of select="high"/>;
	</xsl:template>


</xsl:stylesheet>
