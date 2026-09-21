import os
import sys
from pptx import Presentation
from pptx.util import Inches, Pt
from pptx.enum.text import PP_ALIGN, MSO_ANCHOR
from pptx.dml.color import RGBColor
from pptx.enum.shapes import MSO_SHAPE

def build_presentation():
    prs = Presentation()
    prs.slide_width = Inches(13.333)
    prs.slide_height = Inches(7.5)

    blank_slide_layout = prs.slide_layouts[6] # completely blank layout

    # Colors
    BG_DARK = RGBColor(7, 12, 22)         # Deep Space Navy
    CARD_BG = RGBColor(13, 24, 42)        # Cyber Deck Panel
    CARD_BORDER = RGBColor(28, 52, 88)    # Border Outline
    CYAN_ACCENT = RGBColor(0, 240, 255)   # Neon Cyan
    BLUE_ACCENT = RGBColor(59, 130, 246)  # Electric Blue
    AMBER_ACCENT = RGBColor(245, 158, 11) # Warning Amber
    TEXT_WHITE = RGBColor(248, 250, 252)  # Primary Text
    TEXT_MUTED = RGBColor(148, 163, 184)  # Secondary / Subtitle
    GREEN_ACCENT = RGBColor(34, 197, 94)  # Success Green

    def add_background(slide):
        bg = slide.shapes.add_shape(MSO_SHAPE.RECTANGLE, 0, 0, Inches(13.333), Inches(7.5))
        bg.fill.solid()
        bg.fill.fore_color.rgb = BG_DARK
        bg.line.fill.background()
        return bg

    def add_header(slide, title_text, category_text, slide_num):
        # Category Tag / Breadcrumb
        cat_box = slide.shapes.add_textbox(Inches(0.8), Inches(0.4), Inches(10.0), Inches(0.35))
        tf_cat = cat_box.text_frame
        tf_cat.word_wrap = True
        p_cat = tf_cat.paragraphs[0]
        p_cat.text = category_text.upper()
        p_cat.font.size = Pt(10)
        p_cat.font.bold = True
        p_cat.font.color.rgb = CYAN_ACCENT

        # Main Slide Title
        title_box = slide.shapes.add_textbox(Inches(0.8), Inches(0.68), Inches(10.5), Inches(0.65))
        tf_title = title_box.text_frame
        tf_title.word_wrap = True
        p_title = tf_title.paragraphs[0]
        p_title.text = title_text
        p_title.font.size = Pt(24)
        p_title.font.bold = True
        p_title.font.color.rgb = TEXT_WHITE

        # Slide Number Badge
        num_box = slide.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(11.8), Inches(0.5), Inches(0.75), Inches(0.35))
        num_box.fill.solid()
        num_box.fill.fore_color.rgb = CARD_BG
        num_box.line.color.rgb = CYAN_ACCENT
        num_box.line.width = Pt(1)
        tf_num = num_box.text_frame
        p_num = tf_num.paragraphs[0]
        p_num.text = f"{slide_num:02d}/10"
        p_num.alignment = PP_ALIGN.CENTER
        p_num.font.size = Pt(11)
        p_num.font.bold = True
        p_num.font.color.rgb = CYAN_ACCENT

        # Glowing Divider Line
        line = slide.shapes.add_shape(MSO_SHAPE.RECTANGLE, Inches(0.8), Inches(1.35), Inches(11.75), Inches(0.02))
        line.fill.solid()
        line.fill.fore_color.rgb = CARD_BORDER
        line.line.fill.background()

    def add_card(slide, left, top, width, height, title=None):
        card = slide.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, left, top, width, height)
        card.fill.solid()
        card.fill.fore_color.rgb = CARD_BG
        card.line.color.rgb = CARD_BORDER
        card.line.width = Pt(1)
        if title:
            tb = slide.shapes.add_textbox(left + Inches(0.15), top + Inches(0.15), width - Inches(0.3), Inches(0.4))
            tf = tb.text_frame
            p = tf.paragraphs[0]
            p.text = title.upper()
            p.font.size = Pt(12)
            p.font.bold = True
            p.font.color.rgb = CYAN_ACCENT
        return card

    project_root = r"d:\Unity Project\KittyFly"
    screenshots_dir = os.path.join(project_root, "Assets", "Screenshots")

    # =========================================================================
    # SLIDE 1: TITLE & HERO
    # =========================================================================
    s1 = prs.slides.add_slide(blank_slide_layout)
    add_background(s1)

    # Hero card on left
    c1 = add_card(s1, Inches(0.8), Inches(1.0), Inches(5.8), Inches(5.5))
    tb1 = s1.shapes.add_textbox(Inches(1.1), Inches(1.3), Inches(5.2), Inches(4.8))
    tf1 = tb1.text_frame
    tf1.word_wrap = True

    p = tf1.paragraphs[0]
    p.text = "PROJECT SHOWCASE"
    p.font.size = Pt(12)
    p.font.bold = True
    p.font.color.rgb = CYAN_ACCENT

    p2 = tf1.add_paragraph()
    p2.text = "KITTYFLY"
    p2.font.size = Pt(40)
    p2.font.bold = True
    p2.font.color.rgb = TEXT_WHITE
    p2.space_before = Pt(8)

    p3 = tf1.add_paragraph()
    p3.text = "Odyssey Beyond Orbit"
    p3.font.size = Pt(20)
    p3.font.color.rgb = BLUE_ACCENT
    p3.space_after = Pt(14)

    p4 = tf1.add_paragraph()
    p4.text = "A 2.5D Precision Physics Spacecraft Simulator featuring Newtonian flight mechanics, cybernetic UI, multi-stage hazard navigation, and cinematic visual effects."
    p4.font.size = Pt(13)
    p4.font.color.rgb = TEXT_MUTED
    p4.space_after = Pt(20)

    p5 = tf1.add_paragraph()
    p5.text = "• Engine: Unity 6 (6000.6.2f1) URP"
    p5.font.size = Pt(12)
    p5.font.bold = True
    p5.font.color.rgb = TEXT_WHITE
    p5.space_after = Pt(4)

    p6 = tf1.add_paragraph()
    p6.text = "• Pipeline: Universal Render Pipeline (URP 17.6.0)"
    p6.font.size = Pt(12)
    p6.font.color.rgb = TEXT_MUTED
    p6.space_after = Pt(4)

    p7 = tf1.add_paragraph()
    p7.text = "• Architecture: Rigidbody 2.5D Physics + State Machine UI"
    p7.font.size = Pt(12)
    p7.font.color.rgb = TEXT_MUTED

    # Hero image on right
    hero_img = os.path.join(screenshots_dir, "scifi_menu_main.png")
    if os.path.exists(hero_img):
        s1.shapes.add_picture(hero_img, Inches(6.9), Inches(1.0), Inches(5.65), Inches(5.5))

    # =========================================================================
    # SLIDE 2: CONCEPT & GAMEPLAY LOOP
    # =========================================================================
    s2 = prs.slides.add_slide(blank_slide_layout)
    add_background(s2)
    add_header(s2, "Core Game Concept & Mission Loop", "Game Vision & Pillars", 2)

    # 3 Column Cards
    col_w = Inches(3.7)
    gap = Inches(0.3)
    left_start = Inches(0.8)

    # Col 1: High Concept
    add_card(s2, left_start, Inches(1.6), col_w, Inches(5.2), "01. High Concept")
    tb = s2.shapes.add_textbox(left_start + Inches(0.2), Inches(2.2), col_w - Inches(0.4), Inches(4.3))
    tf = tb.text_frame
    tf.word_wrap = True
    p = tf.paragraphs[0]
    p.text = "Inspired by classic Lunar Lander & precision arcade simulators, KittyFly tests pilot dexterity across hostile alien sectors."
    p.font.size = Pt(12)
    p.font.color.rgb = TEXT_MUTED
    p.space_after = Pt(10)
    p = tf.add_paragraph()
    p.text = "Newtonian Flight Physics"
    p.font.size = Pt(13)
    p.font.bold = True
    p.font.color.rgb = TEXT_WHITE
    p = tf.add_paragraph()
    p.text = "Zero-drag atmospheric inertia demands continuous micro-burst adjustments to avoid terminal velocity crashes."
    p.font.size = Pt(11)
    p.font.color.rgb = TEXT_MUTED
    p.space_after = Pt(10)
    p = tf.add_paragraph()
    p.text = "Damage-Tolerant Hull"
    p.font.size = Pt(13)
    p.font.bold = True
    p.font.color.rgb = TEXT_WHITE
    p = tf.add_paragraph()
    p.text = "100 HP health system allows skilled players to absorb glancing impacts and repair through nano-pickup caches."
    p.font.size = Pt(11)
    p.font.color.rgb = TEXT_MUTED

    # Col 2: The Core Loop
    add_card(s2, left_start + col_w + gap, Inches(1.6), col_w, Inches(5.2), "02. The Mission Loop")
    tb = s2.shapes.add_textbox(left_start + col_w + gap + Inches(0.2), Inches(2.2), col_w - Inches(0.4), Inches(4.3))
    tf = tb.text_frame
    tf.word_wrap = True
    steps = [
        ("STAGE 1: LAUNCH & INGRESS", "Undock from launchpad, fire primary thrusters to conquer local gravity pull."),
        ("STAGE 2: PRECISION VECTORING", "Balance roll torque and burst throttle through tight sci-fi corridors."),
        ("STAGE 3: DYNAMIC HAZARDS", "Time maneuvers past moving cranes, hydraulic crushers, and rotating plasma blades."),
        ("STAGE 4: EXTRACTION DOCKING", "Touch down softly on the glowing target landing pad under safe velocity.")
    ]
    for title, desc in steps:
        p = tf.add_paragraph() if tf.paragraphs[0].text else tf.paragraphs[0]
        p.text = title
        p.font.size = Pt(11)
        p.font.bold = True
        p.font.color.rgb = CYAN_ACCENT
        p2 = tf.add_paragraph()
        p2.text = desc
        p2.font.size = Pt(10)
        p2.font.color.rgb = TEXT_MUTED
        p2.space_after = Pt(8)

    # Col 3: Visual Showcase
    add_card(s2, left_start + (col_w + gap)*2, Inches(1.6), col_w, Inches(5.2), "03. In-Game Demonstration")
    img_loop = os.path.join(screenshots_dir, "scifi_ui_hud_showcase.png")
    if os.path.exists(img_loop):
        s2.shapes.add_picture(img_loop, left_start + (col_w + gap)*2 + Inches(0.15), Inches(2.3), col_w - Inches(0.3), Inches(3.2))
    tb = s2.shapes.add_textbox(left_start + (col_w + gap)*2 + Inches(0.2), Inches(5.6), col_w - Inches(0.4), Inches(1.0))
    tf = tb.text_frame
    tf.word_wrap = True
    p = tf.paragraphs[0]
    p.text = "Real-time 2.5D gameplay showing synchronized HUD telemetry, lighting, and gate navigation."
    p.font.size = Pt(11)
    p.font.color.rgb = TEXT_MUTED

    # =========================================================================
    # SLIDE 3: SPACECRAFT FLIGHT PHYSICS & CONTROLS
    # =========================================================================
    s3 = prs.slides.add_slide(blank_slide_layout)
    add_background(s3)
    add_header(s3, "Spacecraft Flight Mechanics & Rigidbody Physics", "Vehicle Engineering", 3)

    # Left Column: Specs & Code Logic
    add_card(s3, Inches(0.8), Inches(1.6), Inches(6.0), Inches(5.2), "Flight Dynamics Architecture")
    tb = s3.shapes.add_textbox(Inches(1.0), Inches(2.2), Inches(5.6), Inches(4.3))
    tf = tb.text_frame
    tf.word_wrap = True

    specs = [
        ("Kinematic 2.5D Constraints", "Rigidbody with Z-position lock and X/Y rotation freeze. Prevents drifting off 2D gameplay plane while preserving full 3D lighting."),
        ("Newtonian Thrust Vectoring", "Main Thrust: 1000 N force applied along transform.up vector via Rigidbody.AddRelativeForce()."),
        ("Dual-Directional Torque", "Rotation Speed: 100 deg/s with physics torque overrides for responsive steering without gimbal lock."),
        ("Fail-Safe Dual Input Architecture", "Supports Unity Modern Input System actions with seamless fallback to legacy GetKey inputs for 100% hardware compatibility."),
        ("Kinematic Collision Protection", "Detects safe pad landings vs. terrain impacts, checking contact normal and vertical touch speed.")
    ]
    for title, desc in specs:
        p = tf.add_paragraph() if tf.paragraphs[0].text else tf.paragraphs[0]
        p.text = "• " + title
        p.font.size = Pt(12)
        p.font.bold = True
        p.font.color.rgb = CYAN_ACCENT
        p2 = tf.add_paragraph()
        p2.text = desc
        p2.font.size = Pt(11)
        p2.font.color.rgb = TEXT_MUTED
        p2.space_after = Pt(8)

    # Right: Telemetry & In-flight Image
    add_card(s3, Inches(7.1), Inches(1.6), Inches(5.45), Inches(5.2), "Telemetry & Vector Display")
    img_phys = os.path.join(screenshots_dir, "scifi_hud_normal.png")
    if os.path.exists(img_phys):
        s3.shapes.add_picture(img_phys, Inches(7.25), Inches(2.2), Inches(5.15), Inches(4.3))

    # =========================================================================
    # SLIDE 4: MULTI-LAYERED THRUSTER VFX & LIGHTING
    # =========================================================================
    s4 = prs.slides.add_slide(blank_slide_layout)
    add_background(s4)
    add_header(s4, "Multi-Layered Sci-Fi Thruster VFX & Dynamic Lighting", "Visual Effects & Shaders", 4)

    # Top: Visuals (Side by side comparison)
    img_close = os.path.join(screenshots_dir, "scifi_thruster_closeup.png")
    img_act = os.path.join(screenshots_dir, "scifi_thruster_action.png")

    add_card(s4, Inches(0.8), Inches(1.6), Inches(5.7), Inches(3.3), "Macro Plasma Nozzle Ignition")
    if os.path.exists(img_close):
        s4.shapes.add_picture(img_close, Inches(0.95), Inches(2.1), Inches(5.4), Inches(2.65))

    add_card(s4, Inches(6.8), Inches(1.6), Inches(5.7), Inches(3.3), "In-Flight Atmospheric Exhaust Trail")
    if os.path.exists(img_act):
        s4.shapes.add_picture(img_act, Inches(6.95), Inches(2.1), Inches(5.4), Inches(2.65))

    # Bottom: VFX Layer Breakdown Card
    add_card(s4, Inches(0.8), Inches(5.1), Inches(11.7), Inches(1.8), "Multi-Emitter Particle Architecture")
    tb = s4.shapes.add_textbox(Inches(1.0), Inches(5.45), Inches(11.3), Inches(1.3))
    tf = tb.text_frame
    tf.word_wrap = True

    p = tf.paragraphs[0]
    p.text = "1. Core Mach Plume: High-velocity compressed white/cyan ion stream with size-over-lifetime tapering."
    p.font.size = Pt(11)
    p.font.color.rgb = CYAN_ACCENT

    p2 = tf.add_paragraph()
    p2.text = "2. Plasma Outer Envelope: Vibrant cyan (#00F0FF) glow cone delivering distinct sci-fi silhouette and volumetric presence."
    p2.font.size = Pt(11)
    p2.font.color.rgb = TEXT_WHITE

    p3 = tf.add_paragraph()
    p3.text = "3. Radiant Sparks & Embers: Amber/gold micro-particles scattering downward with realistic gravity-assisted drift."
    p3.font.size = Pt(11)
    p3.font.color.rgb = AMBER_ACCENT

    p4 = tf.add_paragraph()
    p4.text = "4. Dynamic Point Light: Synchronized real-time point light that dynamically illuminates ground terrain and canyon walls on burn."
    p4.font.size = Pt(11)
    p4.font.color.rgb = GREEN_ACCENT

    # =========================================================================
    # SLIDE 5: DIEGETIC COCKPIT HUD & TELEMETRY
    # =========================================================================
    s5 = prs.slides.add_slide(blank_slide_layout)
    add_background(s5)
    add_header(s5, "Cyberpunk Diegetic Cockpit HUD & Telemetry", "User Interface & Systems", 5)

    # Left: Features
    add_card(s5, Inches(0.8), Inches(1.6), Inches(5.8), Inches(5.2), "HUD Elements & Pilot Feedback")
    tb = s5.shapes.add_textbox(Inches(1.0), Inches(2.2), Inches(5.4), Inches(4.3))
    tf = tb.text_frame
    tf.word_wrap = True

    hud_feats = [
        ("Tactical Frame & Brackets", "Cyber-cyan hex border overlay (#00F0FF) framing the camera viewport for an immersive cockpit feel."),
        ("Live Velocity & Altitude Readouts", "Real-time telemetry measuring speed (m/s) and vertical elevation, warning pilots of high-speed descent rates."),
        ("Dynamic Thruster Throttle Bar", "Responsive fuel/burn gauge tracking active propellant burn rate in real time."),
        ("Segmented Health Bar (100 HP)", "Color-shifting life bar transitions from Cyber-Cyan (100-60 HP) to Warning Amber (59-30 HP) to Critical Red (<30 HP)."),
        ("Holographic Damage Vignette", "Procedural crimson screen vignette flashes dynamically upon terrain impact, scaling with damage severity.")
    ]
    for title, desc in hud_feats:
        p = tf.add_paragraph() if tf.paragraphs[0].text else tf.paragraphs[0]
        p.text = "• " + title
        p.font.size = Pt(11)
        p.font.bold = True
        p.font.color.rgb = CYAN_ACCENT
        p2 = tf.add_paragraph()
        p2.text = desc
        p2.font.size = Pt(10)
        p2.font.color.rgb = TEXT_MUTED
        p2.space_after = Pt(6)

    # Right: HUD Screenshots (Active HUD + Crash Modal)
    add_card(s5, Inches(6.9), Inches(1.6), Inches(5.6), Inches(2.5), "In-Flight HUD Telemetry")
    img_hud = os.path.join(screenshots_dir, "scifi_hud_normal.png")
    if os.path.exists(img_hud):
        s5.shapes.add_picture(img_hud, Inches(7.05), Inches(2.0), Inches(5.3), Inches(1.95))

    add_card(s5, Inches(6.9), Inches(4.3), Inches(5.6), Inches(2.5), "Mission Failure Debrief Terminal")
    img_crash = os.path.join(screenshots_dir, "scifi_hud_crash.png")
    if os.path.exists(img_crash):
        s5.shapes.add_picture(img_crash, Inches(7.05), Inches(4.7), Inches(5.3), Inches(1.95))

    # =========================================================================
    # SLIDE 6: SECTOR 01 - CANYON FLIGHT
    # =========================================================================
    s6 = prs.slides.add_slide(blank_slide_layout)
    add_background(s6)
    add_header(s6, "Sector 01: Canyon Flight — Atmospheric Ingress", "Level Design & Campaign", 6)

    # Left: Image
    add_card(s6, Inches(0.8), Inches(1.6), Inches(6.5), Inches(5.2), "Sector 01: Canyon Flight View")
    img_s1 = os.path.join(screenshots_dir, "level1_perfect_showcase.png")
    if os.path.exists(img_s1):
        s6.shapes.add_picture(img_s1, Inches(0.95), Inches(2.1), Inches(6.2), Inches(4.5))

    # Right: Card
    add_card(s6, Inches(7.6), Inches(1.6), Inches(4.9), Inches(5.2), "Mission Specifications")
    tb = s6.shapes.add_textbox(Inches(7.8), Inches(2.2), Inches(4.5), Inches(4.3))
    tf = tb.text_frame
    tf.word_wrap = True

    s1_points = [
        ("Environment Theme", "High-altitude atmospheric canyon at twilight with deep cyan ambient fill and moody silhouettes."),
        ("Gameplay Objective", "Master pitch stabilization, low-gravity hover balance, and gentle deceleration curves."),
        ("Key Architecture", "Twin Cyber-Arch Transit Gates glowing with cyan warning markers directing flight traffic."),
        ("Landing Zone", "Elevated neon extraction pad with safe-touchdown velocity threshold sensors."),
        ("Hazard Rating", "Level 1: Low-moderate hazard density designed to establish foundational pilot muscle memory.")
    ]
    for title, desc in s1_points:
        p = tf.add_paragraph() if tf.paragraphs[0].text else tf.paragraphs[0]
        p.text = "► " + title
        p.font.size = Pt(11)
        p.font.bold = True
        p.font.color.rgb = CYAN_ACCENT
        p2 = tf.add_paragraph()
        p2.text = desc
        p2.font.size = Pt(10)
        p2.font.color.rgb = TEXT_MUTED
        p2.space_after = Pt(8)

    # =========================================================================
    # SLIDE 7: SECTOR 02 - INDUSTRIAL REFINERY
    # =========================================================================
    s7 = prs.slides.add_slide(blank_slide_layout)
    add_background(s7)
    add_header(s7, "Sector 02: Industrial Refinery — Moving Hazards", "Level Design & Campaign", 7)

    # Left: Image
    add_card(s7, Inches(0.8), Inches(1.6), Inches(6.5), Inches(5.2), "Kinematic Cargo Crane Obstacle")
    img_s2 = os.path.join(screenshots_dir, "level2_crane_view.png")
    if os.path.exists(img_s2):
        s7.shapes.add_picture(img_s2, Inches(0.95), Inches(2.1), Inches(6.2), Inches(4.5))

    # Right: Specs
    add_card(s7, Inches(7.6), Inches(1.6), Inches(4.9), Inches(5.2), "Sector Mechanics & Obstacles")
    tb = s7.shapes.add_textbox(Inches(7.8), Inches(2.2), Inches(4.5), Inches(4.3))
    tf = tb.text_frame
    tf.word_wrap = True

    s2_points = [
        ("Terrain Complexity", "Asymmetric terrain wave geometry with narrow subterranean ceiling and restricted overhead clearance."),
        ("Dynamic Cargo Crane", "Kinematic industrial arm oscillating across flight lane via Mathf.PingPong() / Sinusoidal wave. Crushes careless ships."),
        ("Nano-Repair Cache (+25 HP)", "Glowing emerald repair pickup floating in the cavern; pilots can take calculated risks to recover hull integrity."),
        ("Narrow Choke Points", "Forces pilots to pause, hover mid-air, and time their forward burn between crane sweeps."),
        ("Hazard Rating", "Level 2: High spatial awareness required; combines moving obstacles with precision hovering.")
    ]
    for title, desc in s2_points:
        p = tf.add_paragraph() if tf.paragraphs[0].text else tf.paragraphs[0]
        p.text = "► " + title
        p.font.size = Pt(11)
        p.font.bold = True
        p.font.color.rgb = AMBER_ACCENT
        p2 = tf.add_paragraph()
        p2.text = desc
        p2.font.size = Pt(10)
        p2.font.color.rgb = TEXT_MUTED
        p2.space_after = Pt(8)

    # =========================================================================
    # SLIDE 8: SECTOR 03 - ORBITAL DEFENSE NEXUS
    # =========================================================================
    s8 = prs.slides.add_slide(blank_slide_layout)
    add_background(s8)
    add_header(s8, "Sector 03: Orbital Defense Nexus — Gauntlet", "Level Design & Campaign", 8)

    # Left: Image
    add_card(s8, Inches(0.8), Inches(1.6), Inches(6.5), Inches(5.2), "360° Rotating Turbine & Piston Gauntlet")
    img_s3 = os.path.join(screenshots_dir, "level3_turbine_view.png")
    if os.path.exists(img_s3):
        s8.shapes.add_picture(img_s3, Inches(0.95), Inches(2.1), Inches(6.2), Inches(4.5))

    # Right: Mechanics
    add_card(s8, Inches(7.6), Inches(1.6), Inches(4.9), Inches(5.2), "Gauntlet Architecture")
    tb = s8.shapes.add_textbox(Inches(7.8), Inches(2.2), Inches(4.5), Inches(4.3))
    tf = tb.text_frame
    tf.word_wrap = True

    s3_points = [
        ("Extreme Gauntlet Architecture", "High-security orbital defense fortress featuring compound synchronized death-traps."),
        ("Synchronized Piston Crushers", "Twin hydraulic vertical pistons moving in counter-cycle; failing to time passage results in instant crushing."),
        ("360° Rotating Plasma Turbine", "Continuous high-speed rotating quad-blade turbine centered in the main extraction shaft. Requires surgical trajectory alignment."),
        ("Multi-Stage Nano Supplies", "Strategic repair pickups distributed before the final orbital ascent shaft."),
        ("Campaign Victory Extraction", "Touching down triggers the Sector Victory screen, unlocking the complete campaign debrief.")
    ]
    for title, desc in s3_points:
        p = tf.add_paragraph() if tf.paragraphs[0].text else tf.paragraphs[0]
        p.text = "► " + title
        p.font.size = Pt(11)
        p.font.bold = True
        p.font.color.rgb = CYAN_ACCENT
        p2 = tf.add_paragraph()
        p2.text = desc
        p2.font.size = Pt(10)
        p2.font.color.rgb = TEXT_MUTED
        p2.space_after = Pt(8)

    # =========================================================================
    # SLIDE 9: INTERACTIVE MENUS & CYBERDECK TERMINAL
    # =========================================================================
    s9 = prs.slides.add_slide(blank_slide_layout)
    add_background(s9)
    add_header(s9, "Sci-Fi Menu Suite & 3D Interactive Diorama", "User Experience Architecture", 9)

    # 3 Column Images / Cards
    add_card(s9, Inches(0.8), Inches(1.6), Inches(3.7), Inches(5.2), "3D Main Menu Diorama")
    img_m1 = os.path.join(screenshots_dir, "scifi_menu_main.png")
    if os.path.exists(img_m1):
        s9.shapes.add_picture(img_m1, Inches(0.95), Inches(2.2), Inches(3.4), Inches(2.2))
    tb = s9.shapes.add_textbox(Inches(0.95), Inches(4.5), Inches(3.4), Inches(2.2))
    tf = tb.text_frame
    tf.word_wrap = True
    p = tf.paragraphs[0]
    p.text = "Interactive 3D Stage"
    p.font.size = Pt(12)
    p.font.bold = True
    p.font.color.rgb = TEXT_WHITE
    p2 = tf.add_paragraph()
    p2.text = "Features real-time floating spaceship with gentle idle levitation physics, glowing platform lights, and cyberdeck command button layout."
    p2.font.size = Pt(10)
    p2.font.color.rgb = TEXT_MUTED

    add_card(s9, Inches(4.8), Inches(1.6), Inches(3.7), Inches(5.2), "Sector Archive Terminal")
    img_m2 = os.path.join(screenshots_dir, "scifi_menu_sectors.png")
    if os.path.exists(img_m2):
        s9.shapes.add_picture(img_m2, Inches(4.95), Inches(2.2), Inches(3.4), Inches(2.2))
    tb = s9.shapes.add_textbox(Inches(4.95), Inches(4.5), Inches(3.4), Inches(2.2))
    tf = tb.text_frame
    tf.word_wrap = True
    p = tf.paragraphs[0]
    p.text = "Direct Mission Selection"
    p.font.size = Pt(12)
    p.font.bold = True
    p.font.color.rgb = TEXT_WHITE
    p2 = tf.add_paragraph()
    p2.text = "Instant jump access to unlocked sectors with real-time sector clearance badges, environmental previews, and hazard level ratings."
    p2.font.size = Pt(10)
    p2.font.color.rgb = TEXT_MUTED

    add_card(s9, Inches(8.8), Inches(1.6), Inches(3.7), Inches(5.2), "Flight Manual & Calibration")
    img_m3 = os.path.join(screenshots_dir, "scifi_menu_manual.png")
    if os.path.exists(img_m3):
        s9.shapes.add_picture(img_m3, Inches(8.95), Inches(2.2), Inches(3.4), Inches(2.2))
    tb = s9.shapes.add_textbox(Inches(8.95), Inches(4.5), Inches(3.4), Inches(2.2))
    tf = tb.text_frame
    tf.word_wrap = True
    p = tf.paragraphs[0]
    p.text = "In-Game Pilot Handbook"
    p.font.size = Pt(12)
    p.font.bold = True
    p.font.color.rgb = TEXT_WHITE
    p2 = tf.add_paragraph()
    p2.text = "Diegetic pilot handbook detailing key bindings, thruster heat management, and customizable audio/sensitivity calibration options."
    p2.font.size = Pt(10)
    p2.font.color.rgb = TEXT_MUTED

    # =========================================================================
    # SLIDE 10: TECHNICAL ARCHITECTURE & ROADMAP
    # =========================================================================
    s10 = prs.slides.add_slide(blank_slide_layout)
    add_background(s10)
    add_header(s10, "Technical Architecture & Future Roadmap", "Summary & Next Steps", 10)

    # Left: Architecture Stack
    add_card(s10, Inches(0.8), Inches(1.6), Inches(5.8), Inches(5.2), "Technology Stack & Performance")
    tb = s10.shapes.add_textbox(Inches(1.0), Inches(2.2), Inches(5.4), Inches(4.3))
    tf = tb.text_frame
    tf.word_wrap = True

    tech_specs = [
        ("Unity 6 Engine & URP Pipeline", "Leverages Universal Render Pipeline with Forward+ rendering and optimized post-processing bloom stacks."),
        ("Zero Garbage Collection Overhead", "Cached component references in Awake() and pre-allocated physics calls ensure silky smooth 120+ FPS flight."),
        ("Cinemachine Dynamic Camera", "Smooth follow camera tracking ship vector with slight look-ahead damping for cinematic velocity perception."),
        ("Modular Component Architecture", "Separated Movement, Health, Oscillator, SciFiHUDController, and SciFiMenuController scripts."),
        ("Multi-Platform Ready", "Decoupled input bindings and responsive Canvas scalers enable immediate porting to Mobile (iOS/Android) and Steam Deck.")
    ]
    for title, desc in tech_specs:
        p = tf.add_paragraph() if tf.paragraphs[0].text else tf.paragraphs[0]
        p.text = "• " + title
        p.font.size = Pt(11)
        p.font.bold = True
        p.font.color.rgb = CYAN_ACCENT
        p2 = tf.add_paragraph()
        p2.text = desc
        p2.font.size = Pt(10)
        p2.font.color.rgb = TEXT_MUTED
        p2.space_after = Pt(6)

    # Right: Future Expansion & Roadmap
    add_card(s10, Inches(6.9), Inches(1.6), Inches(5.65), Inches(5.2), "Strategic Product Roadmap")
    tb2 = s10.shapes.add_textbox(Inches(7.1), Inches(2.2), Inches(5.2), Inches(4.3))
    tf2 = tb2.text_frame
    tf2.word_wrap = True

    roadmap = [
        ("Phase 1: Immersive Spatial Audio", "Dynamic engine sound pitch modulation matching throttle intensity, atmospheric wind shear, and metallic landing thuds."),
        ("Phase 2: Mobile Port & Virtual Gyro", "Dual on-screen thumbsticks with haptic vibration feedback for iOS/Android and Nintendo Switch touchscreens."),
        ("Phase 3: Sector 04 & 05 Expansion", "Gravitational anomalies (Black Holes / Pulsar storms), laser tripwires, and anti-aircraft missile defense turrets."),
        ("Phase 4: Global Leaderboards & Ghost Racing", "Asynchronous ghost recording system letting pilots compete worldwide for fastest clean-flight extraction times.")
    ]
    for title, desc in roadmap:
        p = tf2.add_paragraph() if tf2.paragraphs[0].text else tf2.paragraphs[0]
        p.text = "► " + title
        p.font.size = Pt(11)
        p.font.bold = True
        p.font.color.rgb = GREEN_ACCENT
        p2 = tf2.add_paragraph()
        p2.text = desc
        p2.font.size = Pt(10)
        p2.font.color.rgb = TEXT_MUTED
        p2.space_after = Pt(8)

    # Save presentation
    output_path = os.path.join(project_root, "KittyFly_Showcase_Presentation.pptx")
    prs.save(output_path)
    print(f"Presentation successfully saved to: {output_path}")

if __name__ == "__main__":
    build_presentation()
