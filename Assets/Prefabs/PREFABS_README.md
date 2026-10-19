# Prefabs and Models — How to create Player/Enemy/Weapon/HUD prefabs

هذا الدليل يُرشدك لإنشاء Prefabs جاهزة في Unity من السكربتات الموجودة.

1) Player Prefab
- إنشاء كائن جديد: GameObject → Create Empty → اسمّه "Player".
- أضف مكوّن Capsule أو Mesh يمثل اللاعب كطفل داخل Player (مثلاً "Body").
- أضف مكون CharacterController إلى Player.
- أضف سكربت PlayerController_Advanced إلى Player.
- اجعل Main Camera طفلاً للـPlayer وضعها في موضع (0, 1.7, 0) ثم اسميها "CameraHolder".
- في PlayerController_Advanced عيّن cameraHolder إلى Transform الكاميرا.
- أنشئ Empty child تحت CameraHolder وسمه "WeaponHolder" وضعه أمام الكاميرا.
- إسحب Weapon prefab (انشئه أدناه) إلى WeaponHolder.
- اضبط Tag الخاص بالـPlayer إلى "Player".
- اسحب Player من الـHierarchy إلى مجلد Prefabs في Project لإنشاء Prefab.

2) Weapon Prefab
- أنشئ Empty GameObject وسمّه "Weapon".
- أضف سكربت Weapon_Advanced.
- أضف ParticleSystem كمطفل "MuzzleFlash" واجعله طفلًا للـWeapon.
- أضف AudioSource وعيّن الأصوات إذا كانت متوفرة.
- اضبط fpsCamera على Main Camera في المشهد.
- ضع impactPrefab إذا أردت نموذج تأثير الطلقة (قد يكون Quad بسيط مع مادة).
- اسحب Weapon إلى مجلد Prefabs.

3) Enemy Prefab
- أنشئ Capsule وسمّه Enemy.
- أضف NavMeshAgent.
- أضف سكربت EnemyAI_Advanced.
- أضف نقاط دورية (Empty GameObjects) في المشهد وضعها ضمن Parent يدعى PatrolPoints، ثم اسحبها للمصفوفة patrolPoints في Enemy.
- اضبط Tag الخاص بالـPlayer في المشهد لاستخدامه كهدف.
- اسحب Enemy إلى مجلد Prefabs.

4) HUD Prefab
- GameObject → UI → Canvas.
- داخل Canvas: أنشئ Slider وسمّه HealthBar.
- أنشئ Text لعرض الصحة وText آخر للذخيرة.
- أنشئ Image شفاف يغطي الشاشة بالكامل لعمل DamageFlash (لون أحمر، alpha=0).
- أضف سكربت HUD_Advanced إلى كائن Empty اسمه HUD_Controller وضع المراجع.
- اسحب HUD_Controller إلى مجلد Prefabs.

5) Scene Example (TestScene)
- ضع Ground (Plane) واحتفظ به كـ Navigation Static ثم Bake NavMesh.
- ضع Player prefab على نقطة البداية.
- ضع بعض Enemy prefabs موزعة في أنحاء الخريطة.
- ضع PatrolPoints في المناطق المرغوبة.

القيم الموصى بها
- Player: walk 4.5، run 8.5، jump 1.2، standHeight 2، crouchHeight 1.1
- Weapon: damage 25، fireRate 10، magazineSize 30، reloadTime 2
- Enemy: detectRadius 18، attackRadius 10، health 120

ملاحظة: لا أستطيع رفع ملفات .prefab أو .unity بصيغة ثنائية هنا، لكن التعليمات أعلاه كافية لإنشاء Prefabs يدوياً داخل Unity بسرعة.

إذا تريد، أجهز ملف .unitypackage (أدرجه كمرفق) وأوضح كيف تستورده لتملأ المشروع بالأصول الجاهزة، أو أوجهك خطوة بخطوة عبر شاشة مشاركة (صور/لقطات شاشة) أثناء إنشاء الـPrefabs.