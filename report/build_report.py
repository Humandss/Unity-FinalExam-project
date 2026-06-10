# -*- coding: utf-8 -*-
"""
Render report/report.md into a Korean PDF using reportlab + Malgun Gothic.
Supports a small Markdown subset: #/##/### headings, paragraphs, **bold**,
`inline code`, ``` fenced code blocks, - bullets, N. numbered lists, | tables |.
A page break is inserted before every H1 (except the first).
"""
import os
import re
import html

from reportlab.lib.pagesizes import A4
from reportlab.lib.units import cm
from reportlab.lib import colors
from reportlab.lib.styles import ParagraphStyle
from reportlab.lib.enums import TA_CENTER, TA_LEFT
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import (
    SimpleDocTemplate, Paragraph, Preformatted, Spacer, PageBreak,
    Table, TableStyle, KeepTogether, Flowable,
)

HERE = os.path.dirname(os.path.abspath(__file__))
MD_PATH = os.path.join(HERE, "report.md")
PDF_PATH = os.path.join(HERE, "Refactoring_Report.pdf")

# ---- fonts ----------------------------------------------------------------
FONTS = {
    "Malgun": r"C:\Windows\Fonts\malgun.ttf",
    "MalgunBd": r"C:\Windows\Fonts\malgunbd.ttf",
}
for name, path in FONTS.items():
    pdfmetrics.registerFont(TTFont(name, path))
pdfmetrics.registerFontFamily("Malgun", normal="Malgun", bold="MalgunBd",
                              italic="Malgun", boldItalic="MalgunBd")

BODY = "Malgun"
BOLD = "MalgunBd"
CODE = "Courier"

INK = colors.HexColor("#1a1a1a")
ACCENT = colors.HexColor("#1f4e79")
CODE_BG = colors.HexColor("#f5f5f5")
CODE_BORDER = colors.HexColor("#d9d9d9")
HEAD_BG = colors.HexColor("#1f4e79")
ROW_BG = colors.HexColor("#eef3f8")

# ---- styles ---------------------------------------------------------------
st_title = ParagraphStyle("title", fontName=BOLD, fontSize=24, leading=30,
                          alignment=TA_CENTER, textColor=ACCENT)
st_sub = ParagraphStyle("sub", fontName=BODY, fontSize=12.5, leading=18,
                        alignment=TA_CENTER, textColor=INK)
st_meta = ParagraphStyle("meta", fontName=BODY, fontSize=11.5, leading=20,
                         alignment=TA_CENTER, textColor=INK)
st_h1 = ParagraphStyle("h1", fontName=BOLD, fontSize=17, leading=22,
                       spaceBefore=4, spaceAfter=10, textColor=ACCENT)
st_h2 = ParagraphStyle("h2", fontName=BOLD, fontSize=13.5, leading=18,
                       spaceBefore=12, spaceAfter=6, textColor=INK)
st_h3 = ParagraphStyle("h3", fontName=BOLD, fontSize=11.5, leading=15,
                       spaceBefore=9, spaceAfter=4, textColor=ACCENT)
st_body = ParagraphStyle("body", fontName=BODY, fontSize=10.3, leading=16.5,
                         alignment=TA_LEFT, textColor=INK, spaceAfter=6)
st_bullet = ParagraphStyle("bullet", parent=st_body, leftIndent=16,
                           bulletIndent=4, spaceAfter=3)
st_code = ParagraphStyle("code", fontName=CODE, fontSize=8, leading=10.6,
                         textColor=INK, backColor=CODE_BG,
                         borderColor=CODE_BORDER, borderWidth=0.6,
                         borderPadding=6, leftIndent=2, rightIndent=2,
                         spaceBefore=4, spaceAfter=8)
st_th = ParagraphStyle("th", fontName=BOLD, fontSize=9, leading=12,
                       textColor=colors.white)
st_td = ParagraphStyle("td", fontName=BODY, fontSize=9, leading=12.5,
                       textColor=INK)

PAGE_W, PAGE_H = A4
LMARGIN = RMARGIN = 1.9 * cm
AVAIL_W = PAGE_W - LMARGIN - RMARGIN
FRAME_H = PAGE_H - (2.0 * cm) - (1.8 * cm)  # matches doc top/bottom margins


class FreshPageBreak(Flowable):
    """Move to a fresh page, but do NOT create a blank page when we are already
    at the top of one (avoids the classic PageBreak-after-a-full-page gap)."""

    def wrap(self, availWidth, availHeight):
        # Tolerance (36pt) absorbs the frame's internal padding so that a fresh
        # page is correctly detected as "at top" (no-op) rather than looping.
        if availHeight < FRAME_H - 36.0:
            return (availWidth, availHeight + 1)  # too tall -> forces a page break
        return (0, 0)                              # already at top -> no-op

    def draw(self):
        pass


# ---- inline formatting ----------------------------------------------------
def inline(text):
    """Escape XML, then apply **bold** and `code` to reportlab markup."""
    text = html.escape(text, quote=False)
    text = re.sub(r"\*\*(.+?)\*\*", r"<b>\1</b>", text)
    text = re.sub(r"`(.+?)`", r'<font name="%s">\1</font>' % CODE, text)
    return text


def make_table(rows):
    # rows: list of list[str] (raw markdown cells). first row = header.
    header, body_rows = rows[0], rows[1:]
    ncols = len(header)
    data = [[Paragraph(inline(c), st_th) for c in header]]
    for r in body_rows:
        cells = list(r) + [""] * (ncols - len(r))
        data.append([Paragraph(inline(c), st_td) for c in cells[:ncols]])

    if header and header[0].strip() == "#":
        first = 1.0 * cm
        rest = (AVAIL_W - first) / (ncols - 1)
        widths = [first] + [rest] * (ncols - 1)
    else:
        widths = [AVAIL_W / ncols] * ncols

    t = Table(data, colWidths=widths, repeatRows=1)
    style = [
        ("BACKGROUND", (0, 0), (-1, 0), HEAD_BG),
        ("GRID", (0, 0), (-1, -1), 0.5, CODE_BORDER),
        ("VALIGN", (0, 0), (-1, -1), "TOP"),
        ("LEFTPADDING", (0, 0), (-1, -1), 5),
        ("RIGHTPADDING", (0, 0), (-1, -1), 5),
        ("TOPPADDING", (0, 0), (-1, -1), 4),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 4),
    ]
    for i in range(1, len(data)):
        if i % 2 == 0:
            style.append(("BACKGROUND", (0, i), (-1, i), ROW_BG))
    t.setStyle(TableStyle(style))
    return t


# ---- markdown parsing -----------------------------------------------------
def parse_md(md):
    lines = md.split("\n")
    flow = []
    i, n = 0, len(lines)
    first_h1 = True
    para = []

    def flush_para():
        if para:
            flow.append(Paragraph(inline(" ".join(para).strip()), st_body))
            para.clear()

    while i < n:
        line = lines[i]
        stripped = line.strip()

        # fenced code block
        if stripped.startswith("```"):
            flush_para()
            i += 1
            buf = []
            while i < n and not lines[i].strip().startswith("```"):
                buf.append(lines[i])
                i += 1
            i += 1  # skip closing fence
            # Preformatted does NOT parse markup, so pass code raw (no escaping).
            code = "\n".join(buf)
            flow.append(Preformatted(code, st_code))
            continue

        # table block
        if stripped.startswith("|"):
            flush_para()
            block = []
            while i < n and lines[i].strip().startswith("|"):
                block.append(lines[i].strip())
                i += 1
            rows = []
            for b in block:
                if re.match(r"^\|[\s:\-\|]+\|?$", b):  # separator row
                    continue
                cells = [c.strip() for c in b.strip().strip("|").split("|")]
                rows.append(cells)
            if rows:
                flow.append(Spacer(1, 2))
                flow.append(make_table(rows))
                flow.append(Spacer(1, 6))
            continue

        # headings
        if stripped.startswith("# "):
            flush_para()
            if not first_h1:
                flow.append(FreshPageBreak())
            first_h1 = False
            flow.append(Paragraph(inline(stripped[2:]), st_h1))
            i += 1
            continue
        if stripped.startswith("## "):
            flush_para()
            flow.append(Paragraph(inline(stripped[3:]), st_h2))
            i += 1
            continue
        if stripped.startswith("### "):
            flush_para()
            flow.append(Paragraph(inline(stripped[4:]), st_h3))
            i += 1
            continue

        # list items (bullet or numbered), allow indentation
        indent = len(line) - len(line.lstrip(" "))
        m_b = re.match(r"^- (.*)$", stripped)
        m_n = re.match(r"^(\d+)\. (.*)$", stripped)
        if m_b or m_n:
            flush_para()
            level = min(indent // 3, 2)
            s = ParagraphStyle("li%d" % level, parent=st_bullet,
                               leftIndent=16 + level * 16,
                               bulletIndent=4 + level * 16)
            if m_b:
                flow.append(Paragraph(inline(m_b.group(1)), s, bulletText="•"))
            else:
                flow.append(Paragraph(inline(m_n.group(2)), s,
                                      bulletText=m_n.group(1) + "."))
            i += 1
            continue

        # blank line -> paragraph break
        if stripped == "":
            flush_para()
            i += 1
            continue

        # normal text
        para.append(stripped)
        i += 1

    flush_para()
    return flow


# ---- title page -----------------------------------------------------------
def title_page():
    el = [Spacer(1, 3.2 * cm)]
    el.append(Paragraph("게임 프로그래밍 패턴 — 기말과제", st_sub))
    el.append(Spacer(1, 0.8 * cm))
    el.append(Paragraph("디자인 패턴을 적용한<br/>리팩토링 결과 보고서", st_title))
    el.append(Spacer(1, 0.9 * cm))
    el.append(Paragraph(
        "Unity 3D 슈팅 프로젝트(2개 모듈)에<br/>"
        "State · Object Pool · Observer · Strategy 패턴 적용", st_sub))
    el.append(Spacer(1, 2.6 * cm))
    el.append(Paragraph("이름: ________________&nbsp;&nbsp;&nbsp;&nbsp;학번: ________________", st_meta))
    el.append(Paragraph("제출일: 2026년 6월 ____일", st_meta))
    el.append(Spacer(1, 0.6 * cm))
    el.append(Paragraph("<font size=9 color='#888888'>* 표지의 이름/학번/제출일은 제출 전 직접 기입하세요.</font>", st_meta))
    el.append(PageBreak())
    return el


# ---- footer (page numbers) ------------------------------------------------
def on_page(canvas, doc):
    canvas.saveState()
    canvas.setFont(BODY, 9)
    canvas.setFillColor(colors.HexColor("#888888"))
    if doc.page > 1:
        canvas.drawCentredString(PAGE_W / 2.0, 1.0 * cm, "- %d -" % (doc.page - 1))
    canvas.restoreState()


def main():
    with open(MD_PATH, "r", encoding="utf-8") as f:
        md = f.read()

    story = title_page() + parse_md(md)

    doc = SimpleDocTemplate(
        PDF_PATH, pagesize=A4,
        leftMargin=LMARGIN, rightMargin=RMARGIN,
        topMargin=2.0 * cm, bottomMargin=1.8 * cm,
        title="디자인 패턴을 적용한 리팩토링 보고서",
        author="", subject="Game Programming Patterns Final Report",
    )
    doc.build(story, onFirstPage=on_page, onLaterPages=on_page)
    print("OK ->", PDF_PATH)


if __name__ == "__main__":
    main()
