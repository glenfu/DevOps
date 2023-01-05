<?xml version="1.0" ?><xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"><xsl:output method="text" indent="no"/><xsl:template match="/data"><![CDATA[<div data-wrapper="true" style="font-family:'Segoe UI','Helvetica Neue',sans-serif; font-size:9pt">
<div><span style="background-color:#e7efff">]]><xsl:choose><xsl:when test="knowledgearticle/title"><xsl:value-of select="knowledgearticle/title" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[</span>﻿</div>
</div>
]]></xsl:template></xsl:stylesheet>