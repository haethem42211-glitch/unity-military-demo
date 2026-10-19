# Scene Setup Guide — TestScene

هذا المستند يوضح خطوات إنشاء مشهد اختبار (TestScene) داخل Unity وربط المكوّنات (Player، Weapon، Enemy، HUD).

مراحل سريعة (قم بها داخل Unity بعد استنساخ الريبو):

1. افتح Unity Hub وأنشئ مشروع 3D جديد باستخدام Unity 2022.3 LTS.
2. انسخ مجلد `Assets` من الريبو إلى مجلد المشروع (أو استنساخ الريبو داخل مجلد المشروع).
3. إنشاء المشهد:
   - File → New Scene ثم Save Scene as `Assets/Scenes/TestScene.unity`.

4. إعداد الأرض:
   - GameObject → 3D Object → Plane. سمّه `Ground`.
   - اضبط `Ground` كـ `Navigation Static` (في Inspector) ثم Window → AI → Navigation → Bake لإنشاء NavMesh.

5. إعداد اللاعب (Player):
   - GameObject → 3D Object → Capsule. سمّه `Player`.
   - أضف مكوّن `CharacterController` إلى `Player` (Add Component).
   - أضف السكربت `PlayerController` (Assets/Scripts/PlayerController.cs) إلى `Player`.
   - ضع Tag باسم `Player` على كائن `Player` (في أعلى Inspector).

6. إعداد الكاميرا:
   - اسحب `Main Camera` داخل `Player` (لتصبح طفلًا له).
   - قم بتعديل موضع الكاميرا (مثلاً Position: (0,1.6,0)) بحيث تكون بنظرة تقريبية للاعب.
   - في `PlayerController` ضع مرجع `cameraHolder` إلى Transform الكاميرا.

7. إعداد السلاح (Weapon):
   - داخل `Main Camera` أنشئ Empty GameObject وسمّه `WeaponHolder` وضعه أمام الكاميرا.
   - أضف السكربت `Weapon` إلى `WeaponHolder`.
   - في `Weapon` عيّن `fpsCamera` إلى `Main Camera`، و `hud` لاحقًا إلى كائن الـHUD.
   - (اختياري) أضف ParticleSystem كمرفق لمؤثر الفوهة `muzzleFlash` إذا رغبت.

8. إعداد HUD:
   - GameObject → UI → Canvas.
   - داخل Canvas: Create → UI → Text (Legacy) أو Text - TextMeshPro إذا مثبت.
   - أنشئ ثلاث نصوص: `HealthText`, `AmmoText`, `MessageText` وضعها في أماكن مناسبة.
   - أنشئ Empty GameObject وسمّه `HUD` ثم أضف السكربت `HUDController` إليه.
   - اربط مراجع النصوص في حقول `healthText`, `ammoText`, `messageText` في `HUDController`.
   - في Inspector الخاص بـ `Player` (PlayerController) عيّن حقل `hud` إلى كائن `HUD`.
   - في Inspector الخاص بـ `Weapon` عيّن `hud` أيضًا إلى نفس كائن HUD.

9. إعداد العدو (Enemy):
   - GameObject → 3D Object → Capsule. سمّه `Enemy`.
   - ضع Tag (اختياري) `Enemy` أو استخدم اسمًا فقط.
   - أضف مكوّن `NavMeshAgent` إلى Enemy (Add Component).
   - أضف السكربت `EnemyAI` إلى Enemy.
   - اضبط قيم `detectRadius` و `shootRadius` حسب رغبتك.

10. اختبار اللعب:
    - اضغط Play.
    - استخدم WASD للحركة، الفأرة للتدوير، LeftShift للجري، Space للقفز.
    - اضغط زر الفأرة الأيسر لإطلاق النار، R لإعادة التحميل.

ملحوظات مهمة
- تأكد من عمل Bake للـNavMesh بعد وضع الأرض كـ Navigation Static.
- إذا أردت تحميل مشهد جاهز أو Prefabs، يمكنني تجهيز ملف `.unitypackage` يحتوي Player prefab و Enemy prefab و HUD و TestScene، ورفعه للريبو كملف Release أو برفعه مباشرة في المجلد `Assets/` عبر Git LFS.

إذا تبيّ، أجهز `.unitypackage` وأرفعه هنا — أخبرني إن تفضّل: "unitypackage" أو "Git LFS" لرفع ملفات المشهد والprefabs الكبيرة.
