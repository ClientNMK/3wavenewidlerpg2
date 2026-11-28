# 글자 스타일 견본: 왼쪽 불투명 창 판 위(기본 재질), 오른쪽 필드 위(테두리 재질).
import os
HERE = os.path.dirname(os.path.abspath(__file__)); ROOT = "../../../../../"
OL = "text-shadow:-1px -1px 0 #2A1A14,1px -1px 0 #2A1A14,-1px 1px 0 #2A1A14,1px 1px 0 #2A1A14,0 1px 0 #2A1A14,0 -1px 0 #2A1A14,1px 0 0 #2A1A14,-1px 0 0 #2A1A14"
rows = [("본문", "#FFF3DC", 11, "소라게 소녀 · 공격력 +50"), ("흐린 글", "#C2A888", 9, "보유 효과 · 10마리를 잡으면 도전"), ("강조", "#FFE7A0", 15, "알껍데기 후드 160p"),
        ("금색", "#E6C27A", 13, "Stage 1-5 · 24.6만"), ("좋음", "#9BD65A", 10, "적용 중 · 치명타 확률 +10%"), ("경고", "#E2765E", 11, "0:30 · 포기")]
def line(name, c, s, t, ol):
    return f'<div style="display:flex;align-items:center;height:30px"><span style="width:62px;font:10px Malgun Gothic;color:#998">{name}</span><span style="font-size:{s}px;color:{c};{OL if ol else ""}">{t}</span></div>'
left = "".join(line(*r, False) for r in rows)
left += f'<div style="display:flex;align-items:center;height:36px"><span style="width:62px;font:10px Malgun Gothic;color:#998">진한 글</span><span style="display:inline-block;padding:5px 16px;border-radius:14px;background:#E6C27A;color:#2A1A14;font-size:12px">착용</span>'
left += '<span style="margin-left:10px;display:inline-block;padding:1px 6px;border-radius:7px;background:#E8604E;color:#FFF3DC;font-size:8px">NEW</span><span style="margin-left:6px;font:10px Malgun Gothic;color:#998">← 꼬리표 글자</span></div>'
right = "".join(line(*r, True) for r in rows)
def ol(c, w): return "text-shadow:" + ",".join(f"{x}px {y}px 0 {c}" for x in range(-w, w + 1) for y in range(-w, w + 1) if x or y)
right += f'<div style="position:absolute;left:640px;top:90px;font-size:10px;color:#FFE26B;{ol("#2A1A14", 1)}">81,804</div>'
right += f'<div style="position:absolute;left:600px;top:140px;font-size:16px;color:#FFF4C8;{ol("#961E14", 1)}">5,204,710</div>'
right += f'<div style="position:absolute;left:660px;top:200px;font-size:10px;color:#FFE26B;{ol("#2A1A14", 1)}">223,330</div>'
right += '<div style="position:absolute;left:590px;top:250px;font:10px Malgun Gothic;color:#fff;text-shadow:0 0 3px #000">↑ 데미지: 일반 10pt / 치명타 16pt</div>'
html = f"""<!doctype html><meta charset="utf-8"><style>
@font-face {{ font-family: 'DNF'; src: url('{ROOT}3wavenewidlerpg2/Assets/Resource/Font/DNFBitBitv2.ttf'); }}
body {{ margin:0; background:#20201f; font-family:'DNF'; display:flex; gap:12px; padding:12px; width:880px; }}
.cap {{ color:#ddd; font:12px/24px 'Malgun Gothic'; }}
.box {{ width:330px; height:250px; border-radius:14px; padding:14px 16px; box-sizing:border-box; }}
</style>
<div><div class="cap">① 불투명 창 판 위 → 재질: 기본 (테두리 없음)</div><div class="box" style="background:#3A2419;border:2px solid #E6C27A">{left}</div></div>
<div><div class="cap">② 필드·그림·반투명 HUD 위 → 재질: 테두리 (#2A1A14, 0.3)</div><div class="box" style="position:static;width:480px;background:url('{ROOT}3wavenewidlerpg2/Assets/Resource/Textures/Bg/bg_world_2.png') center 30% / 360px auto">{right}</div></div>"""
open(os.path.join(HERE, "text_style.html"), "w", encoding="utf-8").write(html)
